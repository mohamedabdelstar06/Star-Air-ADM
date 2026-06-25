using StarAirAdm.Domain.Common;

namespace StarAirAdm.Domain.Entities;

public class Checklist : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Preflight, InFlight, Emergency
    public string? CreatedBy { get; set; }

    // Navigation
    public ICollection<ChecklistItem> Items { get; set; } = new List<ChecklistItem>();
}
