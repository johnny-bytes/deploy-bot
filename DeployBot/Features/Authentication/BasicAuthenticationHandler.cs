using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DeployBot.Features.Shared.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DeployBot.Features.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private const string ApiKeyHeaderName = "Authorization";

        private readonly ServiceConfiguration _serviceConfiguration;

        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock, ServiceConfiguration serviceConfiguration)
            : base(options, logger, encoder, clock)
        {
            _serviceConfiguration = serviceConfiguration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue basicAuthHeader))
            {
                return AuthenticateResult.NoResult();
            }

            if (!string.Equals(basicAuthHeader.Scheme, "Basic"))
            {
                return AuthenticateResult.NoResult();
            }

            var headerValueBytes = Convert.FromBase64String(basicAuthHeader.Parameter);
            var userAndPassword = Encoding.UTF8.GetString(headerValueBytes);
            var parts = userAndPassword.Split(':');

            if (parts.Length != 2)
            {
                return AuthenticateResult.Fail("Invalid Basic authentication header");
            }

            string user = parts[0];
            string password = parts[1];

            if (!string.Equals(user, _serviceConfiguration.BasicUsername)
                || (!string.Equals(password, _serviceConfiguration.BasicPassword)))
            {
                return AuthenticateResult.Fail("Invalid user / password.");
            }

            var identity = new ClaimsIdentity(Options.AuthenticationType);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return AuthenticateResult.Success(ticket);
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{_serviceConfiguration.BasicRealm}\", charset=\"UTF-8\"";
            return base.HandleChallengeAsync(properties);
        }
    }
}