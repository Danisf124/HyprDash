using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Globalization;
using System.Net.Http.Headers;
using Spectre.Console;


namespace HyprDash
{
    internal class Program
    {

        //2FKyiv
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var cache = new CachedWeatherService(); // Weather service
            var culture = new CultureInfo("uk-UA"); // for Ukraine region
            DateTime currentDateTime = DateTime.Now;
            var todoList = new TodoDb();

            // Welcome panel and date&time panel
            var welcomePanel = new Panel(
                    new Rows(
                        new Markup($"Привіт [blue]Danisf[/] :3"),
                        new Markup($"Зараз: {currentDateTime.ToString("d MMMM yyyy, dddd, HH:mm", culture)}")))
                        .Border(BoxBorder.Rounded);

            // Weather Tables
            // Table for day
            var weatherForDayTable = new Table()
                .RoundedBorder()
                .BorderColor(Color.White)
                .Title("Погода на сьогодні");

            weatherForDayTable.AddColumn("Час");
            weatherForDayTable.AddColumn("Темп");
            weatherForDayTable.AddColumn("Відчув.");
            weatherForDayTable.AddColumn("Вологість");
            weatherForDayTable.AddColumn("Опади");
            weatherForDayTable.AddColumn("Дощ");
            weatherForDayTable.AddColumn("Сніг");
            weatherForDayTable.AddColumn("Опис");
            weatherForDayTable.AddColumn("Вітер");
            weatherForDayTable.AddColumn("Напрямок");

            await cache.RefreshIfNeed();

            ShowWeatherForDay(cache.Day, weatherForDayTable);



            AnsiConsole.Write(welcomePanel);
            AnsiConsole.Write(weatherForDayTable);


            

          
            // functions

            static void ShowWeatherForWeek(WeatherForWeekResponse weatherForWeekResponse)
            {
                
            }

            static void ShowWeatherForDay(WeatherForDayResponse weatherForDayResponse, Table renderTable)
            {
                var w = weatherForDayResponse.weatherData;
                // Later change on new Markup($"[red]{Markup.Escape(w.Rain[i].ToString())}mm[/]")

                for(int i = 0; i < 24; i++)
                {
                    renderTable.AddRow(
                        new Text(w.Time[i].ToString("HH:mm")),
                        new Text($"{w.Temp[i]}°C"),
                        new Text($"{w.ApparentTemperature[i]}°C"),
                        new Text($"{w.Humidity[i]}%"),
                        new Text($"{w.PrecipitationProbability[i]}%"),
                        new Text($"{w.Rain[i]}mm"),
                        new Text($"{w.Snowfall[i]}cm"),
                        new Text($"{GetWeatherDescription(w.WeatherCode[i])}"),
                        new Text($"{w.WindSpeed[i]}km/h"),
                        new Text($"{GetWindDirection(w.WindDirection[i])}")

                    );

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

