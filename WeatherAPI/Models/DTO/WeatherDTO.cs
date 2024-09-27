namespace WeatherAPI.Models.DTO
{
    public class WeatherDTO
    {
        public string Date { get; set; }

        public string City { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public double Wind { get; set; }
    }
}
