using System.IO;
using System.Security;
using Microsoft.Extensions.Configuration;

namespace DeployBot.Features.Shared.Services
{
    public class ServiceConfiguration
    {
        private const string BasicAuthenticationUserKey = "BasicUsername";
        private const string BasicAuthenticationPasswordKey = "BasicPassword";
        private const string BasicAuthenticationRealmKey = "BasicRealm";

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

            BasicUsername = configuration.GetValue<string>(BasicAuthenticationUserKey);
            BasicPassword = configuration.GetValue<string>(BasicAuthenticationPasswordKey);
            BasicRealm = configuration.GetValue<string>(BasicAuthenticationUserKey);

            EnsureAppDataFolders();
        }

        public string ConnectionString { get; }
        public string DeploymentTemplatesFolder { get; }
        public string ReleaseDropOffFolder { get; }
        public string BasicRealm { get; set; }
        public string BasicUsername { get; }
        public string BasicPassword { get; }
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
