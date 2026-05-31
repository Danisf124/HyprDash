using System;
using System.ComponentModel.DataAnnotations;

//https://api.open-meteo.com/v1/forecast?latitude=52.52&longitude=30.79&hourly=temperature_2m,relative_humidity_2m,apparent_temperature,precipitation_probability,rain,snowfall,weather_code,wind_speed_10m,wind_direction_10m&timezone=Europe%2FMoscow&forecast_days=1

namespace HyprDash
{
    internal class Program
    {

        //2FKyiv
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //Time and data

            DateTime currentDateTime = DateTime.Now;

            // Weather for Day

            var weatherForDayService = new WeatherForDayService();

            var weatherForDayResponse = await weatherForDayService.GetWeatherForDay();

            //Weather for week
            
            // Todo List

        
            //output
            Console.WriteLine(currentDateTime);

            ShowWeatherForDay(weatherForDayResponse);

            // functions

            static void ShowWeatherForDay(WeatherForDayResponse weatherForDayResponse)
            {
                for(int i = 0; i < 24; i++)
                {
                    Console.WriteLine("Weather for day: ");
                    Console.WriteLine($"Time : {weatherForDayResponse.weatherData.Time[i]}");
                    Console.WriteLine($"Temp : {weatherForDayResponse.weatherData.Temp[i]}");
                    Console.WriteLine($"Humidity : {weatherForDayResponse.weatherData.Humidity[i]}");
                    Console.WriteLine($"Apparent temperature : {weatherForDayResponse.weatherData.ApparentTemperature[i]}");
                    Console.WriteLine($"Precipitation probability : {weatherForDayResponse.weatherData.PrecipitationProbability[i]}");
                    Console.WriteLine($"Rain : {weatherForDayResponse.weatherData.Rain[i]}");
                    Console.WriteLine($"Snowfall : {weatherForDayResponse.weatherData.Snowfall[i]}");
                    Console.WriteLine($"Description : {GetWeatherDescription(weatherForDayResponse.weatherData.WeatherCode[i])}");
                    Console.WriteLine($"Wind speed : {weatherForDayResponse.weatherData.WindSpeed[i]}");
                    Console.WriteLine($"Wind direction : {GetWindDirection(weatherForDayResponse.weatherData.WindDirection[i])}");
                    Console.WriteLine();
                }
            }

            static string GetWeatherDescription(int code) => code switch
            {
                0 => "Ясне небо ☀️",
                1 => "Переважно ясно 🌤️",
                2 => "Хмарно ⛅",
                3 => "Похмуро ☁️",
                45 or 48 => "Туман 🌫️",
                51 or 53 or 55 => "Мряка 🌦️",
                61 or 63 or 65 => "Дощ 🌧️",
                71 or 73 or 75 => "Сніг 🌨️",
                80 or 81 or 82 => "Злива 🌧️",
                95 => "Гроза ⛈️",
                96 or 99 => "Гроза з градом 🌩️",
                _ => "Невідомо"
            };

            static string GetWindDirection(int degrees) => degrees switch
            {
                <= 22 or >= 338 => "Північ ↑",
                <= 67  => "Північний схід ↗",
                <= 112 => "Схід →",
                <= 157 => "Південний схід ↘",
                <= 202 => "Південь ↓",
                <= 247 => "Південний захід ↙",
                <= 292 => "Захід ←",
                <= 337 => "Північний захід ↖",
            };
        
        }
    }
}

