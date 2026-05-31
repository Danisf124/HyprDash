using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.VisualBasic;


//https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=30.79&hourly=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation_probability,rain,snowfall,weather_code,wind_speed_10m,wind_direction_10m&timezone=Europe%2FMoscow&forecast_days=1

namespace HyprDash
{
    public class WeatherForDayService
    {
        static public double lat = 50.50215484873138;

        static public double lon = 30.787733726908275;

        public readonly string url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&hourly=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation_probability,rain,snowfall,weather_code,wind_speed_10m,wind_direction_10m&timezone=Europe%2FKyiv&forecast_days=1";


        public async Task<WeatherForDayResponse> GetWeatherForDay()
        {
            var client = new HttpClient();

            try
            {
                //var response = await client.GetAsync(url);
                //response.EnsureSuccessStatusCode();

                var json = await client.GetStringAsync(url);

                var data = JsonSerializer.Deserialize<WeatherForDayResponse>(json);
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