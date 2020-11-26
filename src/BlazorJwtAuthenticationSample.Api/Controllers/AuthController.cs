using System;
using Honamic.Identity.JwtAuthentication;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BlazorJwtAuthenticationSample.Api.Dtos;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BlazorJwtAuthenticationSample.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : AuthController<IdentityUser>
    {
        public AuthController(JwtSignInManager<IdentityUser> jwtSignInManager,
             UserManager<IdentityUser> userManager,
            ILogger<AuthController<IdentityUser>> logger) :
            base(jwtSignInManager, userManager, logger)
        {

        }

        protected override Task<bool> SendTwoFactureCodeAsync(IdentityUser user, string code, string provider)
        {
            throw new NotImplementedException();
        }

        [HttpPost("[action]")]
        public async Task<RegistrationResponseDto> Registration(UserForRegistrationDto newUser)
        {
            var result = new RegistrationResponseDto();

            var user = new IdentityUser { UserName = newUser.Email, Email = newUser.Email };

            var createResult = await _userManager.CreateAsync(user, newUser.Password);

            result.IsSuccessfulRegistration = createResult.Succeeded;

            if (createResult.Succeeded)
            {
                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    result.ConfirmCodeForSample = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                }
            }
            foreach (var error in createResult.Errors)
            {
                result.Errors = createResult.Errors.Select(c => c.Description).ToList();
            }

            return result;
        }

        public record ConfirmEmailInput([Required, EmailAddress] string Email, [Required] string Code);

        [HttpPost("[action]")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailInput confirmEmailInput)
        {
            var user = await _userManager.FindByEmailAsync(confirmEmailInput.Email);

            if (user == null)
            {
                return NotFound();
            }

            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmEmailInput.Code));

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return Ok();
            }

            return StatusCode(500);
        }


        [HttpGet("[action]")]
        public Task<List<IdentityUser>> GetAllUsers()
        {
            return this._userManager.Users.ToListAsync();
        }

    }
}