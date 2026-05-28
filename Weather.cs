using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;



namespace HyprDash
{
    
    internal class Weather
    {
        

        static public string city = "Bovary";

        static public string apiKey = "a63f47ea5062d199ecdd8330f3519c98";

        static public string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=ua"; 

        public async Task GetWeatherFromApi(double temp, int humidity,string description, double windSpeed)
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