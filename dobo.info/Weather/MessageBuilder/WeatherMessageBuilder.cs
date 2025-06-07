using System.Text;
using System.Text.Json;
using dobo.core.Book;
using dobo.core.Extensions;
using dobo.info.MessageBuilder;
using dobo.info.weather.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace dobo.info.weather.MessageBuilder;

public class WeatherMessageBuilder : IMessageBuilder
{
    private readonly HttpClient httpClient = new();
    private readonly Dictionary<string, (string Latitude, string Longitude)> cityCoordinates = new();
    private readonly string? defaultCity;
    private readonly ILogger<WeatherMessageBuilder> logger;

    public WeatherMessageBuilder(IConfiguration configuration, ILogger<WeatherMessageBuilder> logger)
    {
        this.logger = logger;
        
        this.cityCoordinates.Add("Padova", ("45.408", "11.8859"));
        this.cityCoordinates.Add("Padova Est", ("45.408", "11.8859"));

        this.defaultCity = configuration.GetString(AppSettingsKey.WasteCollectionDefaultCity);
    }

    public async Task<string?> Build(string? args)
    {
        var city = args?.Trim().ToLowerInvariant().Split(" ").FirstOrDefault();
        if (string.IsNullOrEmpty(city))
        {
            city = defaultCity;
        }

        if (city.IsNullOrEmpty())
        {
            logger.LogWarning("City is null or empty. Using default city: {DefaultCity}", defaultCity);
            return null;
        }

        if (cityCoordinates.ContainsKey(city) == false)
        {
            return $"Not supported city: \"{city}\". Available cities: {string.Join(", ", cityCoordinates.Keys)}";
        }

        var hourlyData = new[]
        {
            "temperature_2m",
            "rain",
            "wind_speed_10m",
            "relative_humidity_2m",
            "precipitation_probability",
            "precipitation"
        };
        const int forecastDays = 1;
        var url =
            $"https://api.open-meteo.com/v1/forecast?latitude={cityCoordinates[city].Latitude}&longitude={cityCoordinates[city].Longitude}&hourly={string.Join(",", hourlyData)}&forecast_days={forecastDays}";

        var response = await httpClient.GetAsync(url);
        var body= await response.Content.ReadAsStringAsync();
        var weatherResponse = JsonSerializer.Deserialize<WeatherResponse>(body);
        
        if (weatherResponse?.Hourly is {Time.Count: > 0})
        {
            var sb = new StringBuilder();

            var temperatureAverage= weatherResponse.Hourly.Temperature2m.Average();
            var temperatureMax = weatherResponse.Hourly.Temperature2m.Max();
            var temperatureMin = weatherResponse.Hourly.Temperature2m.Min();
            var windMax = weatherResponse.Hourly.WindSpeed10m.Max();
            var humidityMax = weatherResponse.Hourly.RelativeHumidity2m.Max();
            var precipitationProbabilityMax = weatherResponse.Hourly.PrecipitationProbability.Max();
            
            sb.AppendLine($"Average Temperature: {temperatureAverage:F1}°C, Max: {temperatureMax:F1}°C, Min: {temperatureMin:F1}°C\n");
            sb.AppendLine($"Wind Speed (max): {windMax:F1} km/h");
            sb.AppendLine($"Humidity (max): {humidityMax}%");
            sb.AppendLine($"Precipitation Probability (max): {precipitationProbabilityMax}%");

            return sb.ToString();
        }
        
        return null;
    }
}