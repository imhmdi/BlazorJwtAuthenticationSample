using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorJwtAuthenticationSample.Client.HttpServices
{
    public interface IAuthenticationService
    {
        Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration);
      
        Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication);
       
        Task Logout();

        Task<bool> RefreshToken();

    }
}
