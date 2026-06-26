namespace StarAirAdm.Domain.Entities;

public class ChecklistItem : BaseEntity
{
    public int ChecklistId { get; set; }
    public string Description { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsCritical { get; set; }

    // Navigation
    public Checklist Checklist { get; set; } = null!;
}
