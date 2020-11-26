using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorJwtAuthenticationSample.Client.JwtDecoders
{
    public class JwtDecoder : IJwtDecoder
    {
        private readonly ILogger<JwtDecoder> _logger;
        private JwtSecurityTokenHandler jwtSecurityTokenHandler;

        public JwtDecoder(ILogger<JwtDecoder> logger)
        {
            jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            _logger = logger;
        }

        public ClaimsPrincipal GetClaimsPrincipalForJwtToken(string jwtToken, out SecurityToken securityToken)
        {
            try
            {
                return jwtSecurityTokenHandler.ValidateToken(
                    jwtToken,
                    new TokenValidationParameters()
                    {
                        RequireSignedTokens = false,
                        SignatureValidator = (a, b) => jwtSecurityTokenHandler.ReadToken(a),
                        ValidateIssuerSigningKey = false,
                        ValidateActor = false,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateTokenReplay = false
                    },
                    out securityToken);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Unhandling exception");
            }

            securityToken = null;
            return null;
        }
    }
}
