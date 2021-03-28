using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeployBot.Features.Products.Services;
using DeployBot.Infrastructure.Database;

namespace DeployBot.Features.Deployments.Services
{
    public class DeploymentService
    {
        private readonly LiteDbRepository<Deployment> _dbContext;
        private readonly ProductService _productService;

        public DeploymentService(LiteDbRepository<Deployment> dbContext, ProductService productService)
        {
            _dbContext = dbContext;
            _productService = productService;
        }

        public IEnumerable<Deployment> GetDeploymentsByProduct(string productName)
        {
            var product = _productService.GetByName(productName);

            return _dbContext.Query()
                .Where(d => d.ProductId == product.Id)
                .ToList();
        }

        public async Task<Deployment> EnqueueDeploymentForRelease(Release release)
        {
            var deployment = new Deployment
            {
                ReleaseId = release.Id,
                ProductId = release.ProductId,
                StatusChangedOn = DateTime.UtcNow,
                Status = DeploymentStatus.Enqueued
            };

            _dbContext.AddOrUpdate(deployment);

            return deployment;
        }

        public Deployment UpdateStatus(Deployment deployment, DeploymentStatus status)
        {
            deployment.Status = status;
            deployment.StatusChangedOn = DateTime.UtcNow;

            return deployment;
        }

        public Deployment GetNextEnqueuedDeployment()
        {
            return _dbContext.Query()
                .Where(d => d.Status == DeploymentStatus.Enqueued)
                .FirstOrDefault();
        }
    }
}
