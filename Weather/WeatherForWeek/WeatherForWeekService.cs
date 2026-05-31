using System;
using System.Text.Json;
//https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=30.79&daily=weather_code,temperature_2m_max,temperature_2m_min,sunrise,sunset,precipitation_probability_max,wind_speed_10m_max,wind_direction_10m_dominant&timezone=Europe%2FKyiv

namespace HyprDash
{
    public class WeatherForWeekService
    {
        private readonly HttpClient _client;

        static private double _lat = 50.50215484873138;

        static private double _lon = 30.787733726908275;

        private readonly string url =$"https://api.open-meteo.com/v1/forecast?latitude={_lat}&longitude={_lon}&daily=weather_code,temperature_2m_max,temperature_2m_min,sunrise,sunset,precipitation_probability_max,wind_speed_10m_max,wind_direction_10m_dominant&timezone=Europe%2FKyiv";

        public WeatherForWeekService(HttpClient client)
        {
            _client = client;
        }

        public async Task<WeatherForWeekResponse> GetWeatherForWeek()
        {
            var client = _client;

            try
            {
                var json = await client.GetStringAsync(url);

                var data = JsonSerializer.Deserialize<WeatherForWeekResponse>(json);
                return data ?? throw new InvalidOperationException("Порожня відповідь");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Помилка запиту: {e.Message}");
                throw;
            }
            catch (JsonException e)
            {
                Console.WriteLine($"Помилка парсингу: {e.Message}");
                throw;
            }

        }
    }
}