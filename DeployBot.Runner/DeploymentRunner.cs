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
            [Option('d', "cwd", Required = true, HelpText = "Path to the working directory.")]
            public string WorkingDirectory { get; set; }
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

            try
            {
                await ExtractArchive();
                await ExecutePowerShellScript();

                _logger.Information("Deployment completed successfully.");
                return DeploymentStatus.Success;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An unexpected error occurred while trying to deploy.");
                _logger.Information("Deployment failed.");
                return DeploymentStatus.Failed;
            }
        }

        private async Task ExecutePowerShellScript()
        {
            _logger.Information("Executing powershell script {0}.", _options.ScriptPath);
            var scriptContents = await File.ReadAllTextAsync(_options.ScriptPath);

            _logger.Debug("Setting parameters. DeploymentFolder: {0}; Version: {1}.", _options.WorkingDirectory, _options.DeploymentVersion);
            var parameters = new Dictionary<string, string>
            {
                {"DeploymentFolder", _options.WorkingDirectory},
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

        private async Task ExtractArchive()
        {
            _logger.Information("Extracting archive from {0} to {1}.", _options.DeploymentZipPath, _options.WorkingDirectory);
            var releaseFileStream = File.OpenRead(_options.DeploymentZipPath);

            using var zipArchive = new ZipArchive(releaseFileStream, ZipArchiveMode.Read, false);
            await Task.Run(() => zipArchive.ExtractToDirectory(_options.WorkingDirectory));
            _logger.Information("Extracting archive completed.");
        }
    }
}
