using System;
using System.Text.Json.Serialization;
namespace HyprDash
{
    public class WeatherForDayResponse
    {
        [JsonPropertyName("hourly")]
        public WeatherHourly weatherData {get; set;} = new();
    }

    public class WeatherHourly
    {
        [JsonPropertyName("time")]
        public List<DateTime> Time {get; set;}

        [JsonPropertyName("temperature_2m")]
        public List<double> Temp {get; set;}

        [JsonPropertyName("relative_humidity_2m")]
        public List<int> Humidity {get; set;} 

        [JsonPropertyName("apparent_temperature")]
        public List<double> ApparentTemperature {get; set;}

        [JsonPropertyName("precipitation_probability")]
        public List<int> PrecipitationProbability {get; set;}

        [JsonPropertyName("rain")]
        public List<double> Rain {get; set;} //mm

        [JsonPropertyName("snowfall")]
        public List<double> Snowfall {get; set;} //cm

        [JsonPropertyName("weather_code")]
        public List<int> WeatherCode {get; set;}

        [JsonPropertyName("wind_speed_10m")]
        public List<double> WindSpeed {get; set;}

        [JsonPropertyName("wind_direction_10m")]
        public List<int> WindDirection {get; set;}
    }

  
}