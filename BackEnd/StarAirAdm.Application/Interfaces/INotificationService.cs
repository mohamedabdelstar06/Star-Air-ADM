using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StarAirAdm.Application.Interfaces;

public class NotificationDto
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Link { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public interface INotificationService
{
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(string userId);
    Task CreateNotificationAsync(string userId, string message, string? link = null);
}
