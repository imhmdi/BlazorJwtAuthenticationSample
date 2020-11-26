using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorJwtAuthenticationSample.Client.HttpServices;
using Tewr.Blazor.FileReader;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorJwtAuthenticationSample.Client.AuthProviders;
using Microsoft.Extensions.Configuration;
using BlazorJwtAuthenticationSample.Client.HttpServices.WeatherForecasts;
using BlazorJwtAuthenticationSample.Client.JwtDecoders;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
namespace BlazorJwtAuthenticationSample.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

            builder.Services.AddFileReaderService(o => o.UseWasmSharedBuffer = true);
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IJwtDecoder, JwtDecoder>();
            builder.Services.AddScoped<IWeatherForecastHttpService, WeatherForecastHttpService>();

            var apiUrl = builder.Configuration.GetValue<string>("ApiUrl");

            builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
            builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
            builder.Services.AddScoped<IAccessTokenProvider, CustomAccessTokenProvider>();


            builder.Services.AddHttpClient("ServerAPI",
                      client => client.BaseAddress = new Uri(apiUrl))
                    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient("ServerAPI"));

            Console.WriteLine(apiUrl);

            await builder.Build().RunAsync();
        }
    }
}
