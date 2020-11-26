using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorJwtAuthenticationSample.Client.HttpServices
{
    public class RegistrationResponseDto
    {
        public bool IsSuccessfulRegistration { get; set; }

        public string ConfirmCodeForSample { get; set; }

        public IEnumerable<string> Errors { get; set; }
    }
}
