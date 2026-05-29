
using System.Text.Json;

namespace HyprDash;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey = "76204a3676f58f32fc2a7460cbc74058"; 

    public WeatherService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<WeatherResponse> GetHourlyWeather(double lat, double lon)
    {
        string url = $"https://api.openweathermap.org/data/4.0/onecall?lat={lat}&lon={lon}&lang=ua&exclude=current,minutely,daily,alerts&units=metric&appid={_apiKey}";

        try
        {
            HttpResponseMessage responseMessage = await _httpClient.GetAsync(url);

            responseMessage.EnsureSuccessStatusCode();

            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();

            var weatherData = JsonSerializer.Deserialize<WeatherResponse>(jsonResponse);

            return weatherData!;

        }
        catch(HttpRequestException e)
        {
            Console.WriteLine($"\nПомилка HTTP запиту: {e.Message}");
            return null;
        }
    }
}