using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Globalization;
using System.Net.Http.Headers;
using System.Numerics;
using Spectre.Console;
using Spectre.Console.Rendering;


namespace HyprDash
{
    internal class Program
    {
           
        enum ScreenType
        {
            WeatherForDay, // Welcome, Weather for day
            WeatherForWeek, // Weather for week
            TodoList
        }


        static async Task Main()
        {

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var cache = new CachedWeatherService(); // Weather service
            var culture = new CultureInfo("uk-UA"); // for Ukraine region
            var todoList = new TodoDb();

            DateTime currentDateTime = DateTime.Now;
            await cache.RefreshIfNeed();

            // Welcome panel and date&time panel
            var welcomePanel = new Panel(
                    new Rows(
                        new Markup($"Привіт [blue]Danisf[/] :3"),
                        new Markup($"Зараз: {currentDateTime.ToString("d MMMM yyyy, dddd, HH:mm", culture)}")))
                        .Border(BoxBorder.Rounded);

            var layout = new Layout("Root")
            .SplitRows(
                new Layout("Header").Size(5),
                new Layout("Content"),
                new Layout("Footer").Size(6)
                .SplitRows(
                    new Layout("TopFooter").Size(1),
                    new Layout("BottomFooter").Size(1)
                )
            );

            await AnsiConsole.Live(layout)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .StartAsync(async ctx =>
                {
                    ScreenType currentScreen = ScreenType.WeatherForDay;
                    bool isRunning = true;

                   
                    while(isRunning)
                    {
                        //Updating data
                        await cache.RefreshIfNeed();
                        currentDateTime = DateTime.Now;

                        layout["Header"].Update(BuildWelcomePanel(currentDateTime, culture));

                        layout["TopFooter"].Update(new Panel(new Markup("[grey]Використовуйте [yellow]стрілки ← →[/] для перемикання екранів | [red]ESC[/] для виходу[/]")).Border(BoxBorder.None));
                        layout["BottomFooter"].Update(new Text("")); // Empty

                        if(currentScreen == ScreenType.WeatherForDay)
                        {
                            layout["Content"].Update(InitWeatherForDay(cache.Day)).Size(29);
                        }    
                        else if(currentScreen == ScreenType.WeatherForWeek)
                        {
                            layout["Content"].Update(InitWeatherForWeek(cache.Week)).Size(15);
                        }
                        else if(currentScreen == ScreenType.TodoList)
                        {
                            layout["BottomFooter"].Update(new Panel(new Markup("[yellow]A - To add new todo;")));
                        }

                        ctx.Refresh();

                        if(Console.KeyAvailable)
                        {
                            var key = Console.ReadKey(intercept: true).Key;

                            if(key == ConsoleKey.RightArrow)
                            {
                                currentScreen = (ScreenType)(((int)currentScreen + 1) % Enum.GetNames(typeof(ScreenType)).Length);
                            }
                            else if(key == ConsoleKey.LeftArrow)
                            {
                                int totalScreens = Enum.GetNames(typeof(ScreenType)).Length;
                                currentScreen = (ScreenType)(((int)currentScreen - 1 + totalScreens) % totalScreens);
                            }
                            else if(key == ConsoleKey.Escape)
                            {
                                // Exit
                                isRunning = false;
                            }
                            
                        }


                        await Task.Delay(100);
                    }
                });
          
            // functions

            static Table InitWeatherForWeek(WeatherForWeekResponse weatherForWeekResponse)
            {
                var w = weatherForWeekResponse.WeatherForWeekDaily;
                var culture = new CultureInfo("uk-UA");

                var weatherForWeekTable = new Table()
                    .RoundedBorder()
                    .BorderColor(Color.White)
                    .Title("Погода на Тиждень");
            
                weatherForWeekTable.AddColumn("Дата");
                weatherForWeekTable.AddColumn("Темп max");
                weatherForWeekTable.AddColumn("Темп min");
                weatherForWeekTable.AddColumn("Опади");
                weatherForWeekTable.AddColumn("Схід сон.");
                weatherForWeekTable.AddColumn("Захід сон.");
                weatherForWeekTable.AddColumn("Опис");
                weatherForWeekTable.AddColumn("Вітер");
                weatherForWeekTable.AddColumn("Напрямок");

                //7 days
                for(int i = 0; i < 7; i++)
                {
                    weatherForWeekTable.AddRow(
                        new Text(w.Time[i].ToString("dddd, d MMMM", culture)),
                        new Text($"{w.TempMax[i]}°C"),
                        new Text($"{w.TempMin[i]}°C"),
                        new Text($"{w.PrecipitationProbability[i]}%"),
                        new Text($"{w.SunRice[i].ToString("HH:mm")}"),
                        new Text($"{w.SunSet[i].ToString("HH:mm")}"),
                        new Text($"{GetWeatherDescription(w.WeatherCode[i])}"),
                        new Text($"{w.WindSpeed[i]}km/h"),
                        new Text($"{GetWindDirection(w.WindDirection[i])}")
                    );
                }

                return weatherForWeekTable;
            }

            static Table InitWeatherForDay(WeatherForDayResponse weatherForDayResponse)
            {
                var w = weatherForDayResponse.weatherData;
                // Later change on new Markup($"[red]{Markup.Escape(w.Rain[i].ToString())}mm[/]")

                var weatherForDayTable = new Table()
                    .RoundedBorder()
                    .Title("Погода на сьогодні")
                    .BorderColor(Color.White);

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

                //24 hours
                for(int i = 0; i < 24; i++)
                {
                    weatherForDayTable.AddRow(
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

                return weatherForDayTable;
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
        
            Panel BuildWelcomePanel(DateTime dt, CultureInfo culture)
            {
                return new Panel(
                    new Rows(
                        new Markup($"Привіт [blue]Danisf[/] :3"),
                        new Markup($"Зараз: {dt.ToString("d MMMM yyyy, dddd, HH:mm", culture)}")))
                    .Border(BoxBorder.Rounded);
            }

        }
    }
}

