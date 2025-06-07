using System.Text.Json.Serialization;

namespace dobo.info.weather.Entity;

public record Hourly
{
    [JsonPropertyName("time")] public List<string> Time { get; set; }

    [JsonPropertyName("temperature_2m")] public List<double> Temperature2m { get; set; }

    [JsonPropertyName("rain")] public List<double> Rain { get; set; }

    [JsonPropertyName("wind_speed_10m")] public List<double> WindSpeed10m { get; set; }

    [JsonPropertyName("relative_humidity_2m")]
    public List<int> RelativeHumidity2m { get; set; }

    [JsonPropertyName("precipitation_probability")]
    public List<int> PrecipitationProbability { get; set; }

    [JsonPropertyName("precipitation")] public List<double> Precipitation { get; set; }
}

public record HourlyUnits
{
    [JsonPropertyName("time")] public string Time { get; set; }

    [JsonPropertyName("temperature_2m")] public string Temperature2m { get; set; }

    [JsonPropertyName("rain")] public string Rain { get; set; }

    [JsonPropertyName("wind_speed_10m")] public string WindSpeed10m { get; set; }

    [JsonPropertyName("relative_humidity_2m")]
    public string RelativeHumidity2m { get; set; }

    [JsonPropertyName("precipitation_probability")]
    public string PrecipitationProbability { get; set; }

    [JsonPropertyName("precipitation")] public string Precipitation { get; set; }
}

public record WeatherResponse
{
    [JsonPropertyName("latitude")] public double Latitude { get; set; }

    [JsonPropertyName("longitude")] public double Longitude { get; set; }

    [JsonPropertyName("generationtime_ms")]
    public double GenerationtimeMs { get; set; }

    [JsonPropertyName("utc_offset_seconds")]
    public int UtcOffsetSeconds { get; set; }

    [JsonPropertyName("timezone")] public string Timezone { get; set; }

    [JsonPropertyName("timezone_abbreviation")]
    public string TimezoneAbbreviation { get; set; }

    [JsonPropertyName("elevation")] public double Elevation { get; set; }

    [JsonPropertyName("hourly_units")] public HourlyUnits HourlyUnits { get; set; }

    [JsonPropertyName("hourly")] public Hourly Hourly { get; set; }
}