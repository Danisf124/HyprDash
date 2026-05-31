using System;
using System.Text.Json.Serialization;

namespace HyprDash
{
    public class WeatherForWeekResponse
    {
        [JsonPropertyName("daily")]
        public WeatherForWeekDaily WeatherForWeekDaily {get; set;} = new();
    }

    public class WeatherForWeekDaily
    {
        [JsonPropertyName("time")]            
        public List<DateTime> Time {get; set;}

        [JsonPropertyName("weather_code")]
        public List<int> WeatherCode {get; set;}

        [JsonPropertyName("temperature_2m_max")]
        public List<double> TempMax {get; set;}

        [JsonPropertyName("temperature_2m_min")]
        public List<double> TempMin {get; set;}

        [JsonPropertyName("sunrise")]
        public List<DateTime> SunRice {get; set;}

        [JsonPropertyName("sunset")]
        public List<DateTime> SunSet {get; set;}

        [JsonPropertyName("precipitation_probability_max")]
        public List<int> PrecipitationProbability {get; set;}

        [JsonPropertyName("wind_speed_10m_max")]
        public List<double> WindSpeed {get; set;}

        [JsonPropertyName("wind_direction_10m_dominant")]     
        public List<int> WindDirection {get; set;}



    }
}