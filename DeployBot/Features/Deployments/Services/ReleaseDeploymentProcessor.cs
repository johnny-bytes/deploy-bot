using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DeployBot.Features.Applications.Services;
using DeployBot.Features.Shared.Services;
using DeployBot.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeployBot.Features.Deployments.Services
{
    public class ReleaseDeploymentProcessor : IHostedService
    {
        private readonly ILogger<ReleaseDeploymentProcessor> _logger;
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly IServiceProvider _services;
        private bool _isRunning;

        public ReleaseDeploymentProcessor(ILogger<ReleaseDeploymentProcessor> logger, ServiceConfiguration serviceConfiguration, IServiceProvider services)
        {
            _logger = logger;
            _serviceConfiguration = serviceConfiguration;
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _isRunning = true;
            DeploymentLoop();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _isRunning = false;

            return Task.CompletedTask;
        }

        private async void DeploymentLoop()
        {
            using (var scope = _services.CreateScope())
            {
                try
                {
                    await Task.Delay(5000);

                    var deploymentService = scope.ServiceProvider.GetRequiredService<DeploymentService>();
                    var productService = scope.ServiceProvider.GetRequiredService<ProductService>();

                    var enqueuedDeployment = deploymentService.GetNextEnqueuedDeployment();
                    if (enqueuedDeployment != null)
                    {
                        await DeployAsync(enqueuedDeployment, deploymentService, productService);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred during the deployment loop");
                }
            }

            if (_isRunning)
            {
                DeploymentLoop();
            }

        }

        private async Task DeployAsync(Deployment deployment, DeploymentService deploymentService, ProductService productService)
        {
            var deploymentStatus = DeploymentStatus.Success;
            var workingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var product = productService.GetById(deployment.ProductId);

            try
            {
                deploymentService.UpdateStatus(deployment, DeploymentStatus.InProgress);
                Directory.CreateDirectory(workingDirectory);

                var releaseZipPath = Path.Combine(_serviceConfiguration.GetReleaseDropOffFolder(product.Name, deployment.Version),
                    "release.zip");
                var scriptPath = Path.Combine(_serviceConfiguration.DeploymentTemplatesFolder, $"{product.Name}.ps1");

                var arguments = new List<string> {
                    "-z", releaseZipPath,
                    "-s", scriptPath,
                    "-d", workingDirectory,
                    "-v", deployment.Version
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
                await process.WaitForExitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error has occurred while trying to execute deployment runner.");
                deploymentStatus = DeploymentStatus.Failed;
            }
            finally
            {
                try
                {
                    Directory.Delete(workingDirectory, true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error has occurred while trying to delete working directory.");
                }
            }

            deploymentService.UpdateStatus(deployment, deploymentStatus);
        }
    }
}
