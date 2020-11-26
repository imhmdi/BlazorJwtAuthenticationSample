using Microsoft.AspNetCore.Components;
using BlazorJwtAuthenticationSample.Client.HttpServices;
using System.Threading.Tasks;
using BlazorJwtAuthenticationSample.Client.Helpers;
using System.Diagnostics;
using System.Web;

namespace BlazorJwtAuthenticationSample.Client.Pages
{
    public partial class Login
    {
        private UserForAuthenticationDto _userForAuthentication = new UserForAuthenticationDto();

        [Inject]
        public IAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public bool ShowAuthError { get; set; }

        public string Error { get; set; }

        public async Task ExecuteLogin()
        {
            ShowAuthError = false;
            var result = await AuthenticationService.Login(_userForAuthentication);
            if (!result.Succeeded)
            {
                Error = result.Message;
                ShowAuthError = true;
            }
            else
            {
                var querystring = NavigationManager.QueryString();
                var returnUrl = querystring["returnUrl"];
                returnUrl = returnUrl ?? HttpUtility.UrlDecode(returnUrl);
                if (returnUrl.IsLocalUrl())
                {
                    NavigationManager.NavigateTo(returnUrl);
                }
                else
                {
                    NavigationManager.NavigateTo("/");
                }
            }
        }
    }
}
