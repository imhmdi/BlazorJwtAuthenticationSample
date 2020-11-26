using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using BlazorJwtAuthenticationSample.Client.HttpServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorJwtAuthenticationSample.Client.AuthProviders
{
    public class JwtAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly IAccessTokenProvider _provider;
        private readonly IAuthenticationService _authenticationService;
        private readonly NavigationManager _navigation;
        private AccessToken _lastToken;
        private AuthenticationHeaderValue _cachedHeader;
        private Uri[] _authorizedUris;
        private string[] _ignorePaths;
        private readonly string _redirectUrl;

        public JwtAuthorizationMessageHandler(
                IAccessTokenProvider provider,
                NavigationManager navigation,
                IAuthenticationService authenticationService,
                IEnumerable<string> authorizedUrls,
                string[] ignorePaths,
                string redirectUrl)
        {
            _provider = provider;
            _navigation = navigation;
            if (authorizedUrls == null)
            {
                throw new ArgumentNullException(nameof(authorizedUrls));
            }

            _authorizedUris = authorizedUrls.Select(uri => new Uri(uri, UriKind.Absolute)).ToArray();
            if (_authorizedUris.Length == 0)
            {
                throw new ArgumentException("At least one URL must be configured.", nameof(authorizedUrls));
            }
            _ignorePaths = ignorePaths ?? Array.Empty<string>();
            _redirectUrl = redirectUrl;
            _authenticationService = authenticationService;
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {

            if (_authorizedUris.Any(uri => uri.IsBaseOf(request.RequestUri) &&
                    _ignorePaths.All(path => !request.RequestUri.AbsoluteUri.Contains(path, StringComparison.InvariantCultureIgnoreCase))
                    )
                )
            {
                var now = DateTimeOffset.Now;

                if (_lastToken == null || now >= _lastToken.Expires.AddMinutes(-5))
                {
                    var tokenResult = await _provider.RequestAccessToken();

                    if (tokenResult.TryGetToken(out var token))
                    {
                        _lastToken = token;
                        _cachedHeader = new AuthenticationHeaderValue("Bearer", _lastToken.Value);
                    }
                    else
                    {
                        await _authenticationService.Logout();
                        _navigation.NavigateTo(_redirectUrl, true);
                        throw new Exception("invalid token");
                    }
                }

                request.Headers.Authorization = _cachedHeader;
            }
            var sendResult = await base.SendAsync(request, cancellationToken);

            if (sendResult.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await _authenticationService.Logout();
                _navigation.NavigateTo(_redirectUrl, true);
            }

            return sendResult;
        }
    }
}