using StarAirAdm.Domain.Common;

namespace StarAirAdm.Domain.Entities;

public class KneeboardNote : BaseEntity
{
    public string PilotId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Tags { get; set; } // JSON array
    public bool IsSynced { get; set; } = true;

    // Navigation
    public ApplicationUser Pilot { get; set; } = null!;
}
