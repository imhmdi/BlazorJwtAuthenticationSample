using Blazored.LocalStorage;
using BlazorJwtAuthenticationSample.Client.HttpServices;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Threading.Tasks;

namespace BlazorJwtAuthenticationSample.Client.AuthProviders
{
    public class CustomAccessTokenProvider : IAccessTokenProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IAuthenticationService _authenticationService;

        public CustomAccessTokenProvider(ILocalStorageService localStorage, IAuthenticationService authenticationService)
        {
            _localStorage = localStorage;
            _authenticationService = authenticationService;
        }

        public ValueTask<AccessTokenResult> RequestAccessToken()
        {
            return RequestAccessToken(null);
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            var tokenObject = await _localStorage.GetItemAsync<TokenObject>("token");

            if (tokenObject != null && tokenObject.Token != null)
            {
                var now = DateTimeOffset.Now;

                if (now >= tokenObject.tokenExpirationTime.AddSeconds(-30))
                {
                    if (await _authenticationService.RefreshToken())
                    {
                        tokenObject = await _localStorage.GetItemAsync<TokenObject>("token");
                    }
                    else
                    {
                        throw new Exception("refresh token failed");
                    }
                }

                var token = new AccessToken { Value = tokenObject.Token, Expires = tokenObject.tokenExpirationTime };

                var accessTokenResult = new AccessTokenResult(AccessTokenResultStatus.Success, token, "");

                return accessTokenResult;
            }

            return null;

        }
    }
}
