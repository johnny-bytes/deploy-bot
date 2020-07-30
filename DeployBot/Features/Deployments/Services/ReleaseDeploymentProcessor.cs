using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            var workingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            
            try
            {
                Directory.CreateDirectory(workingDirectory);

                var releaseZipPath = Path.Combine(_serviceConfiguration.GetReleaseDropOffFolder(release.Product.Name, release.Version),
                    "release.zip");
                var scriptPath = Path.Combine(_serviceConfiguration.DeploymentTemplatesFolder, $"{release.Product.Name}.ps1");
                
                var arguments = new List<string> {
                    "-z", releaseZipPath, 
                    "-s", scriptPath,
                    "-d", workingDirectory,
                    "-v", release.Version
                };

                var process = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        FileName = _serviceConfiguration.DeploymentRunnerAppPath,
                        UserName = _serviceConfiguration.DeploymentUser,
                        Password = _serviceConfiguration.DeploymentUserPassword,
                        Arguments = string.Join(' ', arguments)
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

            Directory.Delete(workingDirectory, true);

            return await _deploymentService.CreateDeploymentForRelease(release, DateTime.Now, deploymentStatus);
        }
    }
}
