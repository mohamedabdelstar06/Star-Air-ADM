namespace StarAirAdm.Application.Features.Notifications.Commands;

public record MarkAllNotificationsAsReadCommand(string UserId) : IRequest<bool>;

public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand, bool>
{
    private readonly INotificationService _notificationService;

    public MarkAllNotificationsAsReadCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        await _notificationService.MarkAllAsReadAsync(request.UserId);
        return true;
    }
}
