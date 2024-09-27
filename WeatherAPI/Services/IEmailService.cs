namespace WeatherAPI.Services
{
    public interface IEmailService
    {
        Task SendMailAsync(string email, string subject, string message);
    }
}
