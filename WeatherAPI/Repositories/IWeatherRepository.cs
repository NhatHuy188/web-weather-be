using Microsoft.AspNetCore.Identity.Data;
using WeatherAPI.Models.Domains;
using WeatherAPI.Models.DTO;

namespace WeatherAPI.Repositories
{
    public interface IWeatherRepository
    {

        public Task<WeatherDTO?> GetWeatherAsync(string? location = "London");
        public Task<List<WeatherDTO>?> GetForeCastAsync(string? location = "London", int? day = 4);
        public Task<Subscriber?> UnsubscribeAsync(UnsubscribeRequestDTO unsubscribeRequestDTO);
        public Task<List<Subscriber>> GetAllSubscribers();
    }
}
