using StarAirAdm.Domain.Common;
using StarAirAdm.Domain.Enums;
namespace StarAirAdm.Domain.Entities;
public class DecideSession : BaseEntity
{
    public string PilotId { get; set; } = string.Empty;
    public string? Scenario { get; set; }
    public SessionStatus Status { get; set; } = SessionStatus.InProgress;
    public int? FinalRiskScore { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public bool IsSynced { get; set; } = true;
    // Navigation
    public ApplicationUser Pilot { get; set; } = null!;
    public ICollection<DecideStep> Steps { get; set; } = new List<DecideStep>();
}
