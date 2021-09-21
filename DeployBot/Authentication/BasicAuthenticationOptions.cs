using Microsoft.AspNetCore.Authentication;

namespace DeployBot.Authentication
{
    public class BasicAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "Basic";
        public string Scheme => DefaultScheme;
        public string AuthenticationType = DefaultScheme;
    }
}
