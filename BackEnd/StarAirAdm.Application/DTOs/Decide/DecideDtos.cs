namespace StarAirAdm.Application.DTOs.Decide;

public class CreateDecideSessionDto
{
    public string? Scenario { get; set; }
}

public class CreateDecideStepDto
{
    public int StepType { get; set; } // maps to DecideStepType enum
    public string? Input { get; set; }
    public string? Notes { get; set; }
    public string? SelectedAction { get; set; }
}

public class DecideStepResponseDto
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public string StepType { get; set; } = string.Empty;
    public int StepOrder { get; set; }
    public string? Input { get; set; }
    public string? Notes { get; set; }
    public string? SuggestedActions { get; set; }
    public string? SelectedAction { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class DecideSessionResponseDto
{
    public int Id { get; set; }
    public string PilotId { get; set; } = string.Empty;
    public string PilotName { get; set; } = string.Empty;
    public string? Scenario { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? FinalRiskScore { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsSynced { get; set; }
    public List<DecideStepResponseDto> Steps { get; set; } = new();
}
