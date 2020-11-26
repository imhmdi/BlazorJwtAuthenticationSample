using System;

namespace BlazorJwtAuthenticationSample.Client.AuthProviders
{
    public class TokenObject
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public DateTimeOffset tokenExpirationTime { get; set; }
        
        public DateTimeOffset refreshTokenExpirationTime { get; set; }

    }
}
