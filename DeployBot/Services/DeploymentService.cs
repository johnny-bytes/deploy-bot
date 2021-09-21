using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DeployBot.Dto;
using DeployBot.Features.Applications.Services;
using DeployBot.Infrastructure.Database;
using DeployBot.Shared.Configuration;
using LiteDB;

namespace DeployBot.Features.Deployments.Services
{
    public class DeploymentService
    {
        private readonly LiteDbRepository<Deployment> _dbContext;
        private readonly ApplicationService _applicationService;
        private readonly ServiceConfiguration _serviceConfiguration;

        public DeploymentService(LiteDbRepository<Deployment> dbContext, ApplicationService productService, ServiceConfiguration serviceConfiguration)
        {
            _dbContext = dbContext;
            _applicationService = productService;
            _serviceConfiguration = serviceConfiguration;
        }

        public IEnumerable<Deployment> GetDeploymentsByApplicationId(ObjectId applicationId)
        {
            return _dbContext.Query()
                .Where(d => d.ApplicationId == applicationId)
                .ToList();
        }

        public Deployment UpdateStatus(Deployment deployment, DeploymentStatus status)
        {
            deployment.Status = status;
            deployment.StatusChangedOn = DateTime.UtcNow;

            _dbContext.AddOrUpdate(deployment);

            return deployment;
        }

        public Deployment GetById(ObjectId id)
        {
            return _dbContext.GetById(id);
        }

        public bool CheckVersionExists(ObjectId applicationId, string version)
        {
            var application = _applicationService.GetById(applicationId);
            if (application == null) return false;

            return _dbContext.Query()
            .Where(d => d.ApplicationId == applicationId)
            .Where(d => d.Version == version)
            .Exists();
        }

        public Deployment GetNextEnqueuedDeployment()
        {
            return _dbContext.Query()
                .Where(d => d.Status == DeploymentStatus.Enqueued)
                .FirstOrDefault();
        }

        public async Task<Deployment> CreateDeployment(Application application, CreateDeploymentDto createDeploymentDto)
        {
            var deploymentDropOff = _serviceConfiguration.GetDeploymentDropOffFolder(application.Name, createDeploymentDto.Version);
            if (Directory.Exists(deploymentDropOff))
            {
                Directory.Delete(deploymentDropOff, true);
            }

            Directory.CreateDirectory(deploymentDropOff);
            using (var fileStream = File.OpenWrite(Path.Combine(deploymentDropOff, "release.zip")))
            using (var readStream = createDeploymentDto.ReleaseZip.OpenReadStream())
            {
                await readStream.CopyToAsync(fileStream);
            }

            var deployment = new Deployment
            {
                ApplicationId = application.Id,
                Version = createDeploymentDto.Version,
                StatusChangedOn = DateTime.UtcNow,
                Status = DeploymentStatus.Pending
            };

            _dbContext.AddOrUpdate(deployment);

            return deployment;
        }

        public bool DeleteDeployment(Deployment deployment)
        {
            var application = _applicationService.GetById(deployment.ApplicationId);

            var deploymentDropOff = _serviceConfiguration.GetDeploymentDropOffFolder(application.Name, deployment.Version);
            if (Directory.Exists(deploymentDropOff))
            {
                Directory.Delete(deploymentDropOff, true);
            }

            return _dbContext.Remove(deployment);
        }
    }
}
