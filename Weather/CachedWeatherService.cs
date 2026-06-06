using System;

namespace HyprDash
{
    public class CachedWeatherService
    {
        private static readonly HttpClient _httpClient = new();

        private WeatherForDayResponse? _cachedDay;

        private WeatherForWeekResponse? _cachedWeek;

        private DateTime _lastFetched  = DateTime.MinValue;

        private readonly TimeSpan _ttl = TimeSpan.FromMinutes(15);

        public bool IsStale => DateTime.Now - _lastFetched > _ttl;

        public async Task RefreshIfNeed()
        {
            if(!IsStale && _cachedDay != null && _cachedWeek != null) return;
            
            Console.WriteLine("Оновлення даних погоди...");

            var dayService = new WeatherForDayService(_httpClient);
            var weekService = new WeatherForWeekService(_httpClient);

            var dayTask = dayService.GetWeatherForDay();
            var weekTask = weekService.GetWeatherForWeek();

            await Task.WhenAll(dayTask, weekTask);

            _cachedDay = dayTask.Result;
            _cachedWeek = weekTask.Result;

            _lastFetched = DateTime.Now;
        }

        public WeatherForDayResponse Day => 
            _cachedDay ?? throw new InvalidOperationException("Дані не завантажено");
    
        public WeatherForWeekResponse Week => 
            _cachedWeek ?? throw new InvalidOperationException("Дані не завантажено");
    }
}