using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BlazorJwtAuthenticationSample.Client.AuthProviders;

namespace BlazorJwtAuthenticationSample.Client.HttpServices
{
    public class UserForAuthenticationDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }

    public class AuthResponseDto
    { 
        public bool Succeeded { get; set; }
        
        public bool IsLockedOut { get; set; }
        
        public bool IsNotAllowed { get; set; }
        
        public bool RequiresTwoFactor { get; set; }
        
        public TokenObject Tokens { get; set; } 
        
        public string TwoFactorToken { get; set; }
        public string Message { get; set; }
    }

}
