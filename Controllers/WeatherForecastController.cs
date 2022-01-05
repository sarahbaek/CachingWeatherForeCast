using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace CachingWeatherForeCast.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly string weatherForecastGet = "weatherForecastKey";

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            IEnumerable<WeatherForecast> weatherForecastCollection = null;
            if (_memoryCache.TryGetValue(weatherForecastGet, out weatherForecastCollection))
            {
                return weatherForecastCollection;
            }
            else
            {
                weatherForecastCollection = CreateWeatherForecast();
                //save it in the memory
              MemoryCacheEntryOptions cacheOptions =  new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
              _memoryCache.Set(weatherForecastGet, weatherForecastCollection, cacheOptions);
              return weatherForecastCollection;

            }
        }
        // I have set this method to be private and it works. Try on your computer pls
        // Apparently swagger has a little problem recognizing this method, since it has not [HttpGet] or somethin attribute.
        private IEnumerable<WeatherForecast> CreateWeatherForecast()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}
