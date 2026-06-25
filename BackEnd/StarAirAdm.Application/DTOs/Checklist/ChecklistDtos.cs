namespace StarAirAdm.Application.DTOs.Checklist;

public class CreateChecklistDto
{
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Preflight, InFlight, Emergency
    public List<CreateChecklistItemDto> Items { get; set; } = new();
}

public class CreateChecklistItemDto
{
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsCritical { get; set; }
}

public class ChecklistResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ChecklistItemResponseDto> Items { get; set; } = new();
}

public class ChecklistItemResponseDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsCritical { get; set; }
}
