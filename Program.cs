using System;

namespace HyprDash
{
    internal class Program
    {
        static async Task Main()
        {

            //Time and data

            DateTime currentDateTime = DateTime.Now;

            // Weather for Day
            Weather weather = new Weather();

            await weather.GetWeatherFromApi();
            //Weather for week
            
            // Todo List

            
            //output
            Console.WriteLine(currentDateTime);
            Console.WriteLine(weather.temp);
            Console.WriteLine(weather.humidity);
            Console.WriteLine(weather.windSpeed);
            Console.WriteLine(weather.description);
        }
    }
}