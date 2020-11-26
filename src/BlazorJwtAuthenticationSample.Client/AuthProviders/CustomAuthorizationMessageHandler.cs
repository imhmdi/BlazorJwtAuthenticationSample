using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using BlazorJwtAuthenticationSample.Client.HttpServices;

namespace BlazorJwtAuthenticationSample.Client.AuthProviders
{
    public class CustomAuthorizationMessageHandler : JwtAuthorizationMessageHandler
    {
        private readonly IAccessTokenProvider _provider;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomAuthorizationMessageHandler> _logger;

        public CustomAuthorizationMessageHandler(IAccessTokenProvider provider,
            IAuthenticationService authenticationService,
            NavigationManager navigationManager,
            IConfiguration configuration,
            ILogger<CustomAuthorizationMessageHandler> logger)
            : base(provider, navigationManager, authenticationService, new[] { configuration["ApiUrl"] },
                  new[] { "/api/auth/login",
                      "/api/auth/registration",
                      "/api/auth/confirmEmail",
                      "/api/auth/refreshToken"
                  }, "/login")
        {
            _configuration = configuration;
            _logger = logger;

        }

    }
}