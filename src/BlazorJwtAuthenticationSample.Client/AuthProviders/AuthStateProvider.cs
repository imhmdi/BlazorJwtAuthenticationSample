using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorJwtAuthenticationSample.Client.HttpServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorJwtAuthenticationSample.Client.JwtDecoders;

namespace BlazorJwtAuthenticationSample.Client.AuthProviders
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationState _anonymous;
        private readonly IJwtDecoder _jwtDecoder;
        private readonly Lazy<IAuthenticationService> _authenticationService;


        public AuthStateProvider(ILocalStorageService localStorage, IJwtDecoder jwtDecoder, IServiceProvider serviceProvider)
        {
            _localStorage = localStorage;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            _jwtDecoder = jwtDecoder;
            _authenticationService = new Lazy<IAuthenticationService>(() => serviceProvider.GetRequiredService<IAuthenticationService>());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var tokenObject = await _localStorage.GetItemAsync<TokenObject>("token");

            if (tokenObject?.Token != null)
            {
                var now = DateTimeOffset.Now;

                if (now >= tokenObject.tokenExpirationTime.AddSeconds(30))
                {
                    if (await _authenticationService.Value.RefreshToken())
                    {
                        tokenObject = await _localStorage.GetItemAsync<TokenObject>("token");
                    }
                }

                var claimsPrincipal = _jwtDecoder.GetClaimsPrincipalForJwtToken(tokenObject.Token, out _);

                if (claimsPrincipal != null)
                {
                    return new AuthenticationState(claimsPrincipal);
                }
            }

            return _anonymous;
        }

        public void NotifyUserAuthentication(ClaimsPrincipal authenticatedUser)
        {
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}