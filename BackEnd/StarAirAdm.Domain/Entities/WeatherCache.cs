namespace StarAirAdm.Domain.Entities;

public class WeatherCache
{
    public int Id { get; set; }
    public string StationCode { get; set; } = string.Empty;
    public string? MetarRaw { get; set; }
    public string? TafRaw { get; set; }
    public string? ParsedData { get; set; } // JSON
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
}
