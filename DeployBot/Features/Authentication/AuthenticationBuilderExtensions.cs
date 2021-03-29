using System;
using Microsoft.AspNetCore.Authentication;

namespace DeployBot.Features.Authentication
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder authenticationBuilder, Action<BasicAuthenticationOptions> options)
        {
            return authenticationBuilder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(BasicAuthenticationOptions.DefaultScheme, options);
        }
    }
}