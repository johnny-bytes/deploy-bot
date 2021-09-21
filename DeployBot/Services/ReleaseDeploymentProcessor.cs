using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeployBot.Features.Applications.Services;
using DeployBot.Infrastructure.Database;
using DeployBot.Shared.Configuration;
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
                    var applicationService = scope.ServiceProvider.GetRequiredService<ApplicationService>();
                    var deploymentLogService = scope.ServiceProvider.GetRequiredService<DeploymentLogService>();

                    var enqueuedDeployment = deploymentService.GetNextEnqueuedDeployment();
                    if (enqueuedDeployment != null)
                    {
                        await DeployAsync(enqueuedDeployment, deploymentService, applicationService, deploymentLogService);
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

        private async Task DeployAsync(Deployment deployment, DeploymentService deploymentService,
            ApplicationService applicationService, DeploymentLogService deploymentLogService)
        {
            var deploymentStatus = DeploymentStatus.Success;
            var product = applicationService.GetById(deployment.ApplicationId);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("-----BEGIN DEPLOYMENT-----");
            stringBuilder.AppendLine($"Date: {DateTime.Now}");

            try
            {
                deploymentService.UpdateStatus(deployment, DeploymentStatus.InProgress);

                var releaseZipPath = Path.Combine(_serviceConfiguration.GetDeploymentDropOffFolder(product.Name, deployment.Version),
                    "release.zip");
                var scriptPath = Path.Combine(_serviceConfiguration.DeploymentTemplatesFolder, $"{product.Name}.ps1");

                stringBuilder.AppendLine("Arguments:");
                stringBuilder.AppendLine($"Zip path: {releaseZipPath}");
                stringBuilder.AppendLine($"Script path: {scriptPath}");
                stringBuilder.AppendLine($"Version: {deployment.Version}");

                var arguments = new List<string> {
                    "-z", releaseZipPath,
                    "-s", scriptPath,
                    "-v", deployment.Version
                };

                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = _serviceConfiguration.DeploymentRunnerAppPath,
                    Arguments = string.Join(' ', arguments),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };

                if (!string.IsNullOrEmpty(_serviceConfiguration.DeploymentUser))
                {
                    startInfo.Domain = _serviceConfiguration.DeploymentUserDomain;
                    startInfo.UserName = _serviceConfiguration.DeploymentUser;

                    if (_serviceConfiguration.DeploymentUserPassword != null)
                    {
                        startInfo.PasswordInClearText = _serviceConfiguration.DeploymentUserPassword;
                    }

                    _logger.LogInformation("user {0}, Pass: {1}", startInfo.UserName, startInfo.PasswordInClearText);
                }

                var process = new Process
                {
                    StartInfo = startInfo
                };

                process.Start();
                await process.WaitForExitAsync();

                stringBuilder.AppendLine($"-----BEGIN PROCESS STDOUT-----");
                stringBuilder.AppendLine();

                stringBuilder.AppendLine(await process.StandardOutput.ReadToEndAsync());

                stringBuilder.AppendLine($"-----END PROCESS STDOUT-----");
                stringBuilder.AppendLine();

                stringBuilder.AppendLine($"-----BEGIN PROCESS STDERR-----");
                stringBuilder.AppendLine();

                stringBuilder.AppendLine(await process.StandardError.ReadToEndAsync());

                stringBuilder.AppendLine($"-----END PROCESS STDERR-----");
                stringBuilder.AppendLine();

                deploymentStatus = (DeploymentStatus)process.ExitCode;
            }
            catch (Exception ex)
            {
                stringBuilder.AppendLine($"-----BEGIN EXCEPTION IN MAIN-----");
                stringBuilder.AppendLine();

                stringBuilder.AppendLine(ex.ToString());

                stringBuilder.AppendLine($"-----END EXCEPTION IN MAIN-----");
                stringBuilder.AppendLine();

                _logger.LogError(ex, "An unexpected error has occurred while trying to execute deployment runner.");
                deploymentStatus = DeploymentStatus.Failed;
            }

            stringBuilder.AppendLine("-----END DEPLOYMENT-----");

            deploymentService.UpdateStatus(deployment, deploymentStatus);

            try
            {
                deploymentLogService.CreateLogEntry(deployment.Id, stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error has occurred while trying to save deployment logs.");
            }
        }
    }
}
