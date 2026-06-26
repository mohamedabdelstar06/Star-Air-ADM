namespace StarAirAdm.Application.Features.Notifications.Queries;

public record GetMyNotificationsQuery(string UserId) : IRequest<IEnumerable<NotificationDto>>;

public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, IEnumerable<NotificationDto>>
{
    private readonly INotificationService _notificationService;

    public GetMyNotificationsQueryHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken cancellationToken)
    {
        return await _notificationService.GetUserNotificationsAsync(request.UserId);
    }
}
