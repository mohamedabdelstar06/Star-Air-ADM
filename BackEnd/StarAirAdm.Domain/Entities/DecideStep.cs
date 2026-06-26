namespace StarAirAdm.Domain.Entities;

public class DecideStep : BaseEntity
{
    public int SessionId { get; set; }
    public DecideStepType StepType { get; set; }
    public int StepOrder { get; set; }
    public string? Input { get; set; }
    public string? Notes { get; set; }
    public string? SuggestedActions { get; set; } // JSON array
    public string? SelectedAction { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public DecideSession Session { get; set; } = null!;
}
