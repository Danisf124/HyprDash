using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HyprDash;

public class WeatherResponse
{   
    [JsonPropertyName("hourly")]
    public List<HourlyForecast> Hourly {get; set;}
}

public class HourlyForecast
{
    [JsonPropertyName("dt")]
    public long Dt {get; set;} // Unix time

    [JsonPropertyName("temp")]
    public double Temp {get; set;}
    
    [JsonPropertyName("feels_like")]
    public double FeelsLike {get; set;}

    [JsonPropertyName("humidity")]
    public double Humidity {get; set;}

    [JsonPropertyName("wind_speed")]
    public double WindSpeed {get; set;}

    [JsonPropertyName("wind_deg")]
    public double WindDeg {get; set;}

    [JsonPropertyName("weather")]
    public List<WeatherDescription> Weather {get; set;}
}

public class WeatherDescription
{
    [JsonPropertyName("main")]
    public string Main { get; set;}

    [JsonPropertyName("description")]
    public string Description { get; set; }
}