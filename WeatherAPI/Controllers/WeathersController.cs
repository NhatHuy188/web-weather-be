using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeatherAPI.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text.Json;
using WeatherAPI.Controllers.CustomActionFilters;
using WeatherAPI.Data;
using WeatherAPI.Models.Domains;
using WeatherAPI.Models.DTO;
using WeatherAPI.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;
using WeatherAPI.Models;

namespace WeatherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeathersController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly SubcsribersDbContext _dbContext;
        private readonly IWeatherRepository _weatherrepository;
        public WeathersController(IWeatherRepository weatherRepository, SubcsribersDbContext dbContext, IConfiguration configuration, IEmailService emailService)
        {      
           _weatherrepository = weatherRepository;
           _dbContext = dbContext;
           _emailService = emailService;
        }
        //[HttpGet]
        //public async Task<IActionResult> GetWeatherCurrentDay([FromBody] string? location = "London")
        //{
        //    var apiWeatherData = await _weatherrepository.GetWeatherAsync(location);  
        //    if(apiWeatherData == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(apiWeatherData);
        //}

        [HttpGet]
        public async Task<IActionResult> GetForeCast([FromQuery] string? location = "London",[FromQuery] int? day = 5)
        {
            var apiForecastData = await _weatherrepository.GetForeCastAsync(location, day);
            if(apiForecastData == null)
            {
                return NotFound();
            }          
            return Ok(apiForecastData);
        }

        [HttpPost]
        [Route("register")]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            Console.WriteLine("test");
            var checkSubscriber = await _dbContext.Subcribers.FirstOrDefaultAsync(s => s.Email ==registerRequestDTO.Email);
            if(checkSubscriber != null)
            {
                if (checkSubscriber.isConfirmed == true)
                {
                    return BadRequest("Failed!Your account has been existed !");
                }
                else
                {
                    _dbContext.Remove(checkSubscriber);
                    await _dbContext.SaveChangesAsync();
                }
            }
            //if (_dbContext.Subcribers.Any(s => s.Email == registerRequestDTO.Email)){
                
            //}
            var subscriber = new Subscriber
            {
                Email = registerRequestDTO.Email,
                isConfirmed = false,
                VerificationToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64))
            };
            var url = $"{Request.Scheme}://{Request.Host}/api/Weathers/emailconfirm?token={subscriber.VerificationToken}";
            await _emailService.SendMailAsync(subscriber.Email, "Email Confirmation", "Click the link to confirm your email: " + url);
            await _dbContext.AddAsync(subscriber);
            await _dbContext.SaveChangesAsync();
            return Ok("Please confirm your email!");
        }

        [HttpGet]
        [Route("emailconfirm")]
        public async Task<IActionResult> EmailConfirmation([FromQuery] string token)
        { 
            var subscriber = await _dbContext.Subcribers.FirstOrDefaultAsync(s => s.VerificationToken == token);
            if(subscriber == null)
            {
                return BadRequest("Invalid token");
            }
            subscriber.isConfirmed = true;
            await _dbContext.SaveChangesAsync();
            return Ok("Email confirmed");
        }

        [HttpDelete]
        [Route("unsubscribe")]
        [ValidateModel]
        public async Task<IActionResult> Unsubcribe([FromBody] UnsubscribeRequestDTO unsubscribeRequestDTO)
        {
            var subscribe = await _weatherrepository.UnsubscribeAsync(unsubscribeRequestDTO);
            if(subscribe == null)
            {
                return BadRequest("Failed! Your account does not exist !");
            }
            return Ok("Unsubcribe successfully!");
        }
    }
}
