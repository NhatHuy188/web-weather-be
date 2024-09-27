using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models.Domains
{
    public class Subscriber
    {
        [Key]
        public string Email { get; set; }

        public bool isConfirmed { get; set; }

        public string VerificationToken { get; set; }
    }
}
