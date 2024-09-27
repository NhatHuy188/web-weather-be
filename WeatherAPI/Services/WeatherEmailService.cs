#nullable disable
using WeatherAPI.Repositories;

namespace WeatherAPI.Services
{
    public class WeatherEmailService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly IEmailService _emailService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        //private readonly IWeatherRepository _repository;
        public WeatherEmailService(IEmailService emailService, IServiceScopeFactory serviceScopeFactory)
        {
            _emailService = emailService;
            //_repository = repository;   
            _serviceScopeFactory = serviceScopeFactory;
        }
        
        public async Task SendWeatherMailDaily()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _repository = scope.ServiceProvider.GetRequiredService<IWeatherRepository>();

                var subscribers = await _repository.GetAllSubscribers();
                var forecasts = await _repository.GetForeCastAsync("Ha Noi", 1);
                if(forecasts ==  null)
                {
                    Console.WriteLine("null");
                }
                var message = $"Today's weather:\nCity: {forecasts[0].City}:\n Tempurature: {forecasts[0].Temperature}\n" +
                    $"Wind: {forecasts[0].Wind}\n Humidity: {forecasts[0].Humidity}";
     
                foreach (var subscriber in subscribers)
                {
                    await _emailService.SendMailAsync(subscriber.Email, "Daily Weather Email", message);
                }
            }
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async (e) => await SendWeatherMailDaily(),
                            null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stop async");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
