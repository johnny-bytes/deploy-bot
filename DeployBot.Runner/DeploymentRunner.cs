using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using CommandLine;
using DeployBot.Infrastructure.Database;
using Serilog;

namespace DeployBot.Runner
{
    public class DeploymentRunner
    {
        public class Options
        {
            [Option('z', "zip", Required = true, HelpText = "Path to the release.zip file.")]
            public string DeploymentZipPath { get; set; }

            [Option('s', "script", Required = true, HelpText = "Path to the powershell script.")]
            public string ScriptPath { get; set; }

            [Option('v', "version", Required = true, HelpText = "The version that is deployed.")]
            public string DeploymentVersion { get; set; }
        }

        private readonly ILogger _logger;
        private readonly Options _options;

        public DeploymentRunner(ILogger logger, Options options)
        {
            _logger = logger;
            _options = options;
        }

        public async Task<DeploymentStatus> RunDeploymentForRelease()
        {
            _logger.Information("Beginning deployment...");
            string workingDirectory = null;

            try
            {
                workingDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                _logger.Information("Creating working directory. Path: {0}", workingDirectory);

                Directory.CreateDirectory(workingDirectory);

                await ExtractArchive(workingDirectory);
                await ExecutePowerShellScript(workingDirectory);

                _logger.Information("Deployment completed successfully.");
                return DeploymentStatus.Success;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An unexpected error occurred while trying to deploy.");
                _logger.Information("Deployment failed.");
                return DeploymentStatus.Failed;
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(workingDirectory))
                    {
                        Directory.Delete(workingDirectory, true);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "An unexpected error occurred while trying to clean up working directory.");
                }
            }
        }

        private async Task ExecutePowerShellScript(string workingDirectory)
        {
            _logger.Information("Executing powershell script {0}.", _options.ScriptPath);
            var scriptContents = await File.ReadAllTextAsync(_options.ScriptPath);

            _logger.Debug("Setting parameters. DeploymentFolder: {0}; Version: {1}.", workingDirectory, _options.DeploymentVersion);
            var parameters = new Dictionary<string, string>
            {
                {"DeploymentFolder", workingDirectory},
                {"Version", _options.DeploymentVersion}
            };

            using var ps = PowerShell.Create();

            ps.AddScript(scriptContents);
            ps.AddParameters(parameters);

            _logger.Verbose("Invoking script...");
            await ps.InvokeAsync();
            _logger.Verbose("Invoking script completed.");

            if (ps.Streams.Error.Any())
            {
                throw new ScriptingException(ps.InvocationStateInfo.Reason, ps.Streams.Error);
            }
        }

        private async Task ExtractArchive(string workingDirectory)
        {
            _logger.Information("Extracting archive from {0} to {1}.", _options.DeploymentZipPath, workingDirectory);
            var releaseFileStream = File.OpenRead(_options.DeploymentZipPath);

            using var zipArchive = new ZipArchive(releaseFileStream, ZipArchiveMode.Read, false);
            await Task.Run(() => zipArchive.ExtractToDirectory(workingDirectory));
            _logger.Information("Extracting archive completed.");
        }
    }
}
