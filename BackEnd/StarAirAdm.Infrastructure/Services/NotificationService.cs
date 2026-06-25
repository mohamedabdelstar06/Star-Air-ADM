using Microsoft.EntityFrameworkCore;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StarAirAdm.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(string userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Message = n.Message,
            Link = n.Link,
            IsRead = n.IsRead,
            CreatedAt = n.CreatedAt
        });
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null && !notification.IsRead)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var unread = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        if (unread.Any())
        {
            foreach (var n in unread) n.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task CreateNotificationAsync(string userId, string message, string? link = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Message = message,
            Link = link
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }
}
