using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Globalization;
using System.Net.Http.Headers;


namespace HyprDash
{
    internal class Program
    {

        //2FKyiv
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var cache = new CachedWeatherService();
            var culture = new CultureInfo("uk-UA");
            DateTime currentDateTime = DateTime.Now;
            var todoList = new TodoDb();
           
            while(true)
            {
                Console.Clear();

                //Get Weather from API(OpenMeteo) in 15 min.
                await cache.RefreshIfNeed();

                //Time
                Console.WriteLine(currentDateTime.ToString("dd MMMM yyyy, HH:mm", culture));

                // Show weather         
                //ShowWeatherForDay(cache.Day);
                //ShowWeatherForWeek(cache.Week);

                //Todo list
                // Testing correct

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

          
            // functions

            static void ShowWeatherForWeek(WeatherForWeekResponse weatherForWeekResponse)
            {
                Console.WriteLine("Weather for Week: ");
                var culture = new CultureInfo("uk-UA");
                for(int i = 0; i < 7; i++)
                {
                    Console.WriteLine($"Day : {weatherForWeekResponse.WeatherForWeekDaily.Time[i].ToString("dd MMMM",culture)}");
                    Console.WriteLine($"Max temperature : {weatherForWeekResponse.WeatherForWeekDaily.TempMax[i]}°C");
                    Console.WriteLine($"Min temperature : {weatherForWeekResponse.WeatherForWeekDaily.TempMin[i]}°C");
                    Console.WriteLine($"Sun rice : {weatherForWeekResponse.WeatherForWeekDaily.SunRice[i].ToString("HH:mm")}");
                    Console.WriteLine($"Sunset : {weatherForWeekResponse.WeatherForWeekDaily.SunSet[i].ToString("HH:mm")}");
                    Console.WriteLine($"Precipitation probability : {weatherForWeekResponse.WeatherForWeekDaily.PrecipitationProbability[i]}%");
                    Console.WriteLine($"Wind speed : {weatherForWeekResponse.WeatherForWeekDaily.WindSpeed[i]}km/h");
                    Console.WriteLine($"Wind direction : {GetWindDirection(weatherForWeekResponse.WeatherForWeekDaily.WindDirection[i])}");
                    Console.WriteLine($"Description : {GetWeatherDescription(weatherForWeekResponse.WeatherForWeekDaily.WeatherCode[i])}");
                    Console.WriteLine();

                }
            }

            static void ShowWeatherForDay(WeatherForDayResponse weatherForDayResponse)
            {
                Console.WriteLine("Weather for day: ");
                
                for(int i = 0; i < 24; i++)
                {
                    Console.WriteLine($"Time : {weatherForDayResponse.weatherData.Time[i].ToString("HH:mm")}");
                    Console.WriteLine($"Temp : {weatherForDayResponse.weatherData.Temp[i]}°C");
                    Console.WriteLine($"Humidity : {weatherForDayResponse.weatherData.Humidity[i]}%");
                    Console.WriteLine($"Apparent temperature : {weatherForDayResponse.weatherData.ApparentTemperature[i]}°C");
                    Console.WriteLine($"Precipitation probability : {weatherForDayResponse.weatherData.PrecipitationProbability[i]}%");
                    Console.WriteLine($"Rain : {weatherForDayResponse.weatherData.Rain[i]}mm");
                    Console.WriteLine($"Snowfall : {weatherForDayResponse.weatherData.Snowfall[i]}cm");
                    Console.WriteLine($"Description : {GetWeatherDescription(weatherForDayResponse.weatherData.WeatherCode[i])}");
                    Console.WriteLine($"Wind speed : {weatherForDayResponse.weatherData.WindSpeed[i]}km/h");
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

