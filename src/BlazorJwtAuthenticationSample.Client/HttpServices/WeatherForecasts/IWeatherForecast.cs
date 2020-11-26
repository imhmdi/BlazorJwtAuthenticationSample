using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorJwtAuthenticationSample.Client.HttpServices.WeatherForecasts
{
    public interface IWeatherForecastHttpService
    {
        public Task<IEnumerable<WeatherForecastDto>> GetAll();
    }
}
