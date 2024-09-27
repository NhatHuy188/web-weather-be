using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models.DTO
{
    public class RegisterRequestDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
