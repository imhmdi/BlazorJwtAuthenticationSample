using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using BlazorJwtAuthenticationSample.Client.AuthProviders;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Diagnostics;
using BlazorJwtAuthenticationSample.Client.JwtDecoders;
using System;
using Microsoft.IdentityModel.Tokens;

namespace BlazorJwtAuthenticationSample.Client.HttpServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly Lazy<HttpClient> _client;
        private readonly IJwtDecoder _jwtDecoder;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorage;
        public AuthenticationService(IHttpClientFactory httpClientFactory,
            AuthenticationStateProvider authStateProvider,
            ILocalStorageService localStorage,
            IJwtDecoder jwtDecoder)
        {
            _httpClientFactory = httpClientFactory;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
            _jwtDecoder = jwtDecoder;
            _client = new Lazy<HttpClient>(() => _httpClientFactory.CreateClient("ServerAPI"));
        }

        public async Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration)
        {
            var registrationResult = await _client.Value.PostAsJsonAsync("/api/auth/registration", userForRegistration);
            var result = await registrationResult.Content.ReadFromJsonAsync<RegistrationResponseDto>();

            if (registrationResult.IsSuccessStatusCode)
            {
                if (result.IsSuccessfulRegistration && result.ConfirmCodeForSample != null)
                {
                    var confirmEmailResult = await _client.Value.PostAsJsonAsync<object>("/api/auth/confirmEmail", new
                    {
                        Code = result.ConfirmCodeForSample,
                        Email = userForRegistration.Email
                    });
                }

            }

            return result;
        }

        public async Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication)
        {
            var authResult = await _client.Value.PostAsJsonAsync("/api/auth/login", userForAuthentication);

            var result = await authResult.Content.ReadFromJsonAsync<AuthResponseDto>();

            if (!authResult.IsSuccessStatusCode || !result.Succeeded)
                return result;

            await SaveToken(result);

            var claimsPrincipal = _jwtDecoder.GetClaimsPrincipalForJwtToken(result.Tokens.Token, out SecurityToken _);

            ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(claimsPrincipal);

            return result;
        }

        public async Task Logout()
        {
            if (await _localStorage.ContainKeyAsync("token"))
            {
                await _localStorage.RemoveItemAsync("token");
            }

            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

        public async Task<bool> RefreshToken()
        {
            var tokenObject = await _localStorage.GetItemAsync<TokenObject>("token");

            var now = DateTimeOffset.Now;

            if (now >= tokenObject.refreshTokenExpirationTime.AddSeconds(-5))
            {
                await Logout();
                return false;
            }

            var refreshResult = await _client.Value.PostAsJsonAsync("/api/auth/refreshToken", new
            {
                Token = tokenObject.Token,
                RefreshToken = tokenObject.RefreshToken
            });

            var result = await refreshResult.Content.ReadFromJsonAsync<AuthResponseDto>();

            if (!refreshResult.IsSuccessStatusCode)
            {
                await Logout();
                //throw new ApplicationException("Something went wrong during the refresh token action");
                return false;
            }

            await SaveToken(result);

            return true;
        }

        private async Task SaveToken(AuthResponseDto result)
        {
            await _localStorage.SetItemAsync("token", result.Tokens);
        }
    }
}
