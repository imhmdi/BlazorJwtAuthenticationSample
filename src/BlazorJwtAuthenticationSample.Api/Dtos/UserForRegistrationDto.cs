using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorJwtAuthenticationSample.Api.Dtos
{
    public class UserForRegistrationDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegistrationResponseDto
    {
        public bool IsSuccessfulRegistration { get; set; }

        public string ConfirmCodeForSample { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}
