
using System.Net.Mail;
using System.Net;
using System;
using WeatherAPI.Models.Domains;

namespace WeatherAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMailAsync(string email, string subject, string message)
        {
            SmtpClient client = new SmtpClient(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"]))
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_configuration["Smtp:FromEmail"], _configuration["Smtp:Password"]),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp:FromEmail"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true 
            };

            mailMessage.To.Add(email);
            await client.SendMailAsync(mailMessage);
        }
    }
}
