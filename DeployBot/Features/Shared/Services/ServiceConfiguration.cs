using System.IO;
using System.Security;
using Microsoft.Extensions.Configuration;

namespace DeployBot.Features.Shared.Services
{
    public class ServiceConfiguration
    {
        private const string ApiKeyConfigurationKey = "ApiKey";
        private const string DataFolderConfigurationKey = "AppDataFolder";

        private const string DeploymentUserConfigurationKey = "DeploymentUser";
        private const string DeploymentUserPasswordConfigurationKey = "DeploymentUserPassword";
        private const string DeploymentRunnerAppPathConfigurationKey = "DeployRunnerAppPath";

        public ServiceConfiguration(IConfiguration configuration)
        {
            var appData = configuration.GetValue<string>(DataFolderConfigurationKey);

            DeploymentUser = configuration.GetValue<string>(DeploymentUserConfigurationKey);
            var password = configuration.GetValue<string>(DeploymentUserPasswordConfigurationKey);

            if (!string.IsNullOrEmpty(password))
            {
                DeploymentUserPassword = new SecureString();

                foreach (var character in password)
                {
                    DeploymentUserPassword.AppendChar(character);
                }
            }

            ReleaseDropOffFolder = Path.Combine(appData, "releases");
            DeploymentTemplatesFolder = Path.Combine(appData, "scripts");
            ConnectionString = $"Filename={Path.Combine(appData, "DeployBot2.db")};Connection=shared";

            DeploymentRunnerAppPath = configuration.GetValue<string>(DeploymentRunnerAppPathConfigurationKey);

            ApiKey = configuration.GetValue<string>(ApiKeyConfigurationKey);

            EnsureAppDataFolders();
        }

        public string ConnectionString { get; }
        public string DeploymentTemplatesFolder { get; }
        public string ReleaseDropOffFolder { get; }
        public string ApiKey { get; }
        public string DeploymentUser { get; }
        public string DeploymentRunnerAppPath { get; set; }
        public SecureString DeploymentUserPassword { get; }

        public string GetReleaseDropOffFolder(string product, string version)
        {
            var productVersionReleaseFolder = Path.Combine(ReleaseDropOffFolder, product, version);
            if (!Directory.Exists(productVersionReleaseFolder))
            {
                Directory.CreateDirectory(productVersionReleaseFolder);
            }

            return productVersionReleaseFolder;
        }

        private void EnsureAppDataFolders()
        {
            if (!Directory.Exists(DeploymentTemplatesFolder))
            {
                Directory.CreateDirectory(DeploymentTemplatesFolder);
            }

            if (!Directory.Exists(ReleaseDropOffFolder))
            {
                Directory.CreateDirectory(ReleaseDropOffFolder);
            }
        }
    }
}
