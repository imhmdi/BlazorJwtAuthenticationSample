using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace BlazorJwtAuthenticationSample.Client.JwtDecoders
{
    public interface IJwtDecoder
    {
        ClaimsPrincipal GetClaimsPrincipalForJwtToken(string jwtToken, out SecurityToken securityToken);
    }
}
