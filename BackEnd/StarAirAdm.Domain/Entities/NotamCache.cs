namespace StarAirAdm.Domain.Entities;

public class NotamCache
{
    public int Id { get; set; }
    public string AirportCode { get; set; } = string.Empty;
    public string NotamText { get; set; } = string.Empty;
    public string? Category { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
}
