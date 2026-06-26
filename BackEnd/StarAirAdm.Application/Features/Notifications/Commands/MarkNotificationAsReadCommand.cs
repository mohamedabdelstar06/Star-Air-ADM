namespace StarAirAdm.Application.Features.Notifications.Commands;

public record MarkNotificationAsReadCommand(int NotificationId) : IRequest<bool>;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    private readonly INotificationService _notificationService;

    public MarkNotificationAsReadCommandHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        await _notificationService.MarkAsReadAsync(request.NotificationId);
        return true;
    }
}
