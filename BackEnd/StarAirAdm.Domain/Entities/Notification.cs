using System;
using System.ComponentModel.DataAnnotations;

namespace StarAirAdm.Domain.Entities;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser? User { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public string? Link { get; set; }

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
