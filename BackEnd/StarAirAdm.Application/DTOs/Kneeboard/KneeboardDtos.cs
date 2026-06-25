namespace StarAirAdm.Application.DTOs.Kneeboard;

public class CreateKneeboardNoteDto
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public bool IsSynced { get; set; } = true;
}

public class KneeboardNoteResponseDto
{
    public int Id { get; set; }
    public string PilotId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public bool IsSynced { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
