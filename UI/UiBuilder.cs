using Spectre.Console;
using System.Globalization;

namespace HyprDash
{
    public class UiBuilder
    {
        static public Table BuildWeatherForWeek(WeatherForWeekResponse weatherForWeekResponse)
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

        static public Table BuildWeatherForDay(WeatherForDayResponse weatherForDayResponse)
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

        static public Table BuildTodoTable(TodoList todoList)
        {
            var list = todoList.TodoLists;

            var todoTable = new Table()
            .RoundedBorder()
            .BorderColor(Color.White)
            .Title("Todo List");

            todoTable.AddColumn("Номер");
            todoTable.AddColumn("Назва");
            todoTable.AddColumn("Статус");
            todoTable.AddColumn("Дата");

            for(int i = 0; i < todoList.TodoLists.Count; i++)
            {
                todoTable.AddRow(
                    new Text($"{list[i].Id}"),
                    new Text($"{list[i].Title}"),
                    new Text($"{(list[i].IsCompleted ? "Виконано" : "Не виконано")}"),
                    new Text($"{list[i].CreatedAt.ToString("dd/MM/yy, HH:mm")}")
                );
            }

            return todoTable;
        }

        static public string GetWeatherDescription(int code) => code switch
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

        static public string GetWindDirection(int degrees) => degrees switch
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
        
        static public Panel BuildWelcomePanel(DateTime dt, CultureInfo culture)
        {
            return new Panel(
                new Rows(
                    new Markup($"Привіт [blue]Danisf[/] :3"),
                    new Markup($"Зараз: {dt.ToString("d MMMM yyyy, dddd, HH:mm", culture)}")))
                .Border(BoxBorder.Rounded);
        }
    }
}