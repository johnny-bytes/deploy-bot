using System.IO;
using Microsoft.Extensions.Configuration;

namespace DeployBot.Features.Shared.Services
{
    public class ServiceConfiguration
    {
        private const string ApiKeyConfigurationKey = "ApiKey";
        private const string DataFolderConfigurationKey = "AppDataFolder";

        public ServiceConfiguration(IConfiguration configuration)
        {
            var appData = configuration.GetValue<string>(DataFolderConfigurationKey);

            ReleaseDropOffFolder = Path.Combine(appData, "releases");
            DeploymentTemplatesFolder = Path.Combine(appData, "scripts");
            ConnectionString = $"Data Source={Path.Combine(appData, "DeployBot.db")}";

            ApiKey = configuration.GetValue<string>(ApiKeyConfigurationKey);
            
            EnsureAppDataFolders();
        }

        public string ConnectionString { get; }

        public string DeploymentTemplatesFolder { get; }

        public string ReleaseDropOffFolder { get; }

        public string ApiKey { get; }

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
