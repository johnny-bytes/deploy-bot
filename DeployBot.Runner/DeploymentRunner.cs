using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Threading.Tasks;
using CommandLine;
using DeployBot.Infrastructure.Database;
using Microsoft.Extensions.Logging;

namespace DeployBot.Runner
{
    public class DeploymentRunner
    {
        public class Options
        {
            [Option('z', "zip", Required = true, HelpText = "Path to the release.zip file.")]
            public string ReleaseZipPath { get; set; }
            [Option('s', "script", Required = true, HelpText = "Path to the powershell script.")]
            public string ScriptPath { get; set; }
            [Option('d', "cwd", Required = true, HelpText = "Path to the working directory.")]
            public string WorkingDirectory { get; set; }
            [Option('v', "version", Required = true, HelpText = "The version that is deployed.")]
            public string ReleaseVersion { get; set; }
        }

        private readonly ILogger<DeploymentRunner> _logger;
        private readonly Options _options;

        public DeploymentRunner(ILogger<DeploymentRunner> logger, Options options)
        {
            _logger = logger;
            _options = options;
        }

        public async Task<DeploymentStatus> RunDeploymentForRelease()
        {
            try
            {
                await ExtractArchive();
                await ExecutePowerShellScript();

                return DeploymentStatus.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while trying to deploy release.");
                return DeploymentStatus.Failed;
            }
        }

        private async Task ExecutePowerShellScript()
        {
            var scriptContents = await File.ReadAllTextAsync(_options.ScriptPath);

            var parameters = new Dictionary<string, string>
            {
                {"ReleaseFolder", _options.WorkingDirectory},
                {"Version", _options.ReleaseVersion}
            };

            using var ps = PowerShell.Create();

            ps.AddScript(scriptContents);
            ps.AddParameters(parameters);

            await ps.InvokeAsync();
        }

        private async Task ExtractArchive()
        {
            var releaseFileStream = File.OpenRead(_options.ReleaseZipPath);

            using var zipArchive = new ZipArchive(releaseFileStream, ZipArchiveMode.Read, false);
            await Task.Run(() => zipArchive.ExtractToDirectory(_options.WorkingDirectory));
        }
    }
}
