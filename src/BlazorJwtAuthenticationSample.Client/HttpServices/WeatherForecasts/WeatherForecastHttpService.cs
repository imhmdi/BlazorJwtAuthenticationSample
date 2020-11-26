using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorJwtAuthenticationSample.Client.HttpServices.WeatherForecasts
{
    public class WeatherForecastHttpService : IWeatherForecastHttpService
    {
        private readonly HttpClient _client;

        public WeatherForecastHttpService(HttpClient client)
        {
            _client = client;
        }

        public async Task<IEnumerable<WeatherForecastDto>> GetAll()
        {
            var result = await _client.GetAsync("/api/WeatherForecast");
            var content = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<List<WeatherForecastDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return null;
        }
    }
}
