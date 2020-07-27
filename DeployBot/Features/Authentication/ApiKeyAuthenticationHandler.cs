using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DeployBot.Features.Shared.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeployBot.Features.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string ApiKeyHeaderName = "X-Api-Key";

        private readonly ServiceConfiguration _serviceConfiguration;

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ServiceConfiguration serviceConfiguration) 
            : base(options, logger, encoder, clock)
        {
            _serviceConfiguration = serviceConfiguration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeader))
            {
                return AuthenticateResult.NoResult();
            }

            if (!string.Equals(_serviceConfiguration.ApiKey, apiKeyHeader, StringComparison.Ordinal))
            {
                return AuthenticateResult.NoResult();
            }

            var identity = new ClaimsIdentity(Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }
    }
}