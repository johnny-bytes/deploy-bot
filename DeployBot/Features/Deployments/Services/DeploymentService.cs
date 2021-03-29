using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeployBot.Features.Deployments.DTO;
using DeployBot.Features.Applications.Services;
using DeployBot.Features.Shared.Services;
using DeployBot.Infrastructure.Database;
using LiteDB;

namespace DeployBot.Features.Deployments.Services
{
    public class DeploymentService
    {
        private readonly LiteDbRepository<Deployment> _dbContext;
        private readonly ProductService _productService;
        private readonly ServiceConfiguration _serviceConfiguration;

        public DeploymentService(LiteDbRepository<Deployment> dbContext, ProductService productService, ServiceConfiguration serviceConfiguration)
        {
            _dbContext = dbContext;
            _productService = productService;
            _serviceConfiguration = serviceConfiguration;
        }

        public IEnumerable<Deployment> GetDeploymentsByProduct(string productName)
        {
            var product = _productService.GetByName(productName);

            return _dbContext.Query()
                .Where(d => d.ProductId == product.Id)
                .ToList();
        }

        public Deployment UpdateStatus(Deployment deployment, DeploymentStatus status)
        {
            deployment.Status = status;
            deployment.StatusChangedOn = DateTime.UtcNow;

            _dbContext.AddOrUpdate(deployment);

            return deployment;
        }

        public Deployment GetById(string id)
        {
            return _dbContext.GetById(new ObjectId(id));
        }

        public Deployment GetNextEnqueuedDeployment()
        {
            return _dbContext.Query()
                .Where(d => d.Status == DeploymentStatus.Enqueued)
                .FirstOrDefault();
        }

        public async Task<Deployment> CreateDeployment(string productName, CreateDeploymentDto createReleaseDto)
        {
            var product = _productService.GetOrAddByName(productName);

            var releaseDropOff = _serviceConfiguration.GetReleaseDropOffFolder(product.Name, createReleaseDto.Version);
            if (Directory.Exists(releaseDropOff))
            {
                Directory.Delete(releaseDropOff, true);
            }

            Directory.CreateDirectory(releaseDropOff);
            using (var fileStream = File.OpenWrite(Path.Combine(releaseDropOff, "release.zip")))
            using (var readStream = createReleaseDto.ReleaseZip.OpenReadStream())
            {
                await readStream.CopyToAsync(fileStream);
            }

            var deployment = _dbContext.Query().Where(r => r.Version == createReleaseDto.Version).FirstOrDefault() ?? new Deployment
            {
                ProductId = product.Id,
                Version = createReleaseDto.Version
            };

            _dbContext.AddOrUpdate(deployment);

            return deployment;
        }
    }
}
