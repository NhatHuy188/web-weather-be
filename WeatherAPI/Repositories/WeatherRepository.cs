using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using WeatherAPI.Data;
using WeatherAPI.Models.Domains;
using WeatherAPI.Models.DTO;

namespace WeatherAPI.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly SubcsribersDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public WeatherRepository(IHttpClientFactory httpClientFactory, SubcsribersDbContext dbContext, IConfiguration configuration )
        {
            _httpClientFactory = httpClientFactory;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<List<Subscriber>> GetAllSubscribers()
        {
            return await _dbContext.Subcribers.Where(s => s.isConfirmed == true).ToListAsync();
        }

        public async Task<List<WeatherDTO>?> GetForeCastAsync(string? location = "London", int? day = 5)
        {
            var _baseUrl = _configuration["WeatherApi:baseUrl"];
            var _apiKey = _configuration["WeatherApi:apiKey"];
            string pattern = @"^[A-Za-zÀ-ỹ]+(?:[ '\-][A-Za-zÀ-ỹ]+)*$";
            if (!Regex.IsMatch(location, pattern))
            {
                throw new Exception("Invalid!");
            }
            string url = $"{_baseUrl}/forecast.json?key={_apiKey}&q={location}&days={day}";
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error!");
            }
            var data = await response.Content.ReadAsStringAsync();
            var apiWeatherData = JsonConvert.DeserializeObject<dynamic>(data);
            if (apiWeatherData == null)
            {
                return null;
            }
            var forecasts = new List<WeatherDTO>();          
            foreach (var forecast in apiWeatherData.forecast.forecastday)
            {
                var weather = new WeatherDTO()
                {
                    Date = forecast.date,
                    City = location,
                    Temperature = forecast.day.avgtemp_c,
                    Humidity = forecast.day.avghumidity,
                    Wind = forecast.day.maxwind_mph,
                };    
                forecasts.Add(weather);
            }
            return forecasts;
        }

        public async Task<WeatherDTO?> GetWeatherAsync(string? location = "London")
        {
            var _baseUrl = _configuration["WeatherApi:baseUrl"];
            var _apiKey = _configuration["WeatherApi:apiKey"];
            string pattern = @"^[a-zA-Z]+$";
            if(!Regex.IsMatch(location, pattern))
            {
                throw new Exception("Invalid!");
            }
            string url = $"{_baseUrl}/current.json?key={_apiKey}&q={location}";
            var client = _httpClientFactory.CreateClient();

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error!");
            }
            var data = await response.Content.ReadAsStringAsync();
            var apiWeatherData = JsonConvert.DeserializeObject<dynamic>(data);
            if (apiWeatherData == null)
            {
                return null;
            }
            var weather = new WeatherDTO
            {
                Date = apiWeatherData.location.localtime,
                City = apiWeatherData.location.name,
                Temperature = apiWeatherData.current.temp_c,
                Humidity = apiWeatherData.current.humidity,
                Wind = apiWeatherData.current.wind_mph,
            };
            return weather;
        }

        public async Task<Subscriber?> UnsubscribeAsync(UnsubscribeRequestDTO unsubscribeRequestDTO)
        {
            var checkSubscriber = await _dbContext.Subcribers.FirstOrDefaultAsync(s => s.Email == unsubscribeRequestDTO.Email);
            if(checkSubscriber == null)
            {
                return null;
            }
            _dbContext.Remove(checkSubscriber);
            await _dbContext.SaveChangesAsync();
            return checkSubscriber;
        }
    }
}
