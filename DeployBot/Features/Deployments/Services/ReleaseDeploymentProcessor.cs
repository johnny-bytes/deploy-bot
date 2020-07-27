using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DeployBot.Features.Shared.Services;
using DeployBot.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace DeployBot.Features.Deployments.Services
{
    public class ReleaseDeploymentProcessor
    {
        private readonly ILogger<ReleaseDeploymentProcessor> _logger;
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly DeploymentService _deploymentService;

        public ReleaseDeploymentProcessor(ILogger<ReleaseDeploymentProcessor> logger, ServiceConfiguration serviceConfiguration, DeploymentService deploymentService)
        {
            _logger = logger;
            _serviceConfiguration = serviceConfiguration;
            _deploymentService = deploymentService;
        }

        public async Task<Deployment> RunAsync(Release release)
        {
            DeploymentStatus deploymentStatus;

            try
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        WorkingDirectory = "workingDirectory",
                        FileName = @"fileName",
                        UserName = _serviceConfiguration.DeploymentUser,
                        Password = _serviceConfiguration.DeploymentUserPassword
                    }
                };

                process.Start(); 
                process.WaitForExit();
            
                deploymentStatus = (DeploymentStatus)process.ExitCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error has occurred while trying to execute deployment runner.");
                deploymentStatus = DeploymentStatus.Failed;
            }

            return await _deploymentService.CreateDeploymentForRelease(release, DateTime.Now, deploymentStatus);
        }
    }
}
