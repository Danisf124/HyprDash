using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;


// Full link:https://api.openweathermap.org/data/4.0/onecall/current?lat=50.502027476871525&lon=30.787643652818044&appid=1a98b2ff7a2f07899629cc7b3933ac09

namespace HyprDash
{
    
    internal class Weather
    {
        
        // lat, len for Brovary

        public const double LAT = 50.502027476871525;

        public const double LON = 30.787643652818044;

        public const string APIKEY = "1a98b2ff7a2f07899629cc7b3933ac09";

        static public string url = $"https://api.openweathermap.org/data/4.0/onecall/current?lat={LAT}&lon={LON}&units=metric&lang=ua&appid={APIKEY}";

        public double temp;
        public int humidity;
        public string description;
        public double windSpeed;

        public Weather()
        {}

        public async Task GetWeatherFromApi()
        {
            try
            {
                using HttpClient client = new HttpClient();

                string response = await client.GetStringAsync(url);

                using JsonDocument doc = JsonDocument.Parse(response);
                JsonElement root = doc.RootElement;

                JsonElement mainInfo = root.GetProperty("main");
                temp = mainInfo.GetProperty("temp").GetDouble();
                humidity = mainInfo.GetProperty("humidity").GetInt32();

                description = root.GetProperty("weather")[0].GetProperty("description").GetString()!;

                windSpeed = root.GetProperty("wind").GetProperty("speed").GetDouble();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Помилка з'єднання з API: {ex.Message}");
            }
        }

    }

}