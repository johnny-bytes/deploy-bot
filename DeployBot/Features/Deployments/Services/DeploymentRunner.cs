using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Threading.Tasks;
using DeployBot.Features.Shared.Services;
using DeployBot.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace DeployBot.Features.Deployments.Services
{
    public class DeploymentRunner
    {
        private readonly ILogger<DeploymentRunner> _logger;
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly DeploymentService _deploymentService;

        public DeploymentRunner(ILogger<DeploymentRunner> logger, ServiceConfiguration serviceConfiguration, DeploymentService deploymentService)
        {
            _logger = logger;
            _serviceConfiguration = serviceConfiguration;
            _deploymentService = deploymentService;
        }

        public async Task<Deployment> RunDeploymentForRelease(Release release)
        {
            var workingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            try
            {
                Directory.CreateDirectory(workingDirectory);

                using (var zipArchive = new ZipArchive(File.OpenRead(Path.Combine(
                    _serviceConfiguration.GetReleaseDropOffFolder(release.Product.Name, release.Version),
                    "release.zip")), ZipArchiveMode.Read, false))
                {
                    zipArchive.ExtractToDirectory(workingDirectory);
                }

                var scriptPath = Path.Combine(_serviceConfiguration.DeploymentTemplatesFolder,
                    $"{release.Product.Name}.ps1");
                var scriptContents = await File.ReadAllTextAsync(scriptPath);

                var parameters = new Dictionary<string, string>
                {
                    {"ReleaseFolder", workingDirectory},
                    {"Version", release.Version}
                };

                using (var ps = PowerShell.Create())
                {
                    ps.AddScript(scriptContents);
                    ps.AddParameters(parameters);

                    await ps.InvokeAsync();
                }

                return await _deploymentService.CreateDeploymentForRelease(release, DateTime.Now, DeploymentStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while trying to deploy release with id {0}.", release.Id);
                return await _deploymentService.CreateDeploymentForRelease(release, DateTime.Now, DeploymentStatus.Failed);
            }
            finally
            {
                if (Directory.Exists(workingDirectory))
                {
                    Directory.Delete(workingDirectory, true);
                }
            }
        }
    }
}
