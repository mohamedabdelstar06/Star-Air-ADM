namespace StarAirAdm.Application.Features.ImSafe.Commands;

public record DeleteImSafeCommand(int Id, string RequestingUserId) : IRequest<bool>;

public class DeleteImSafeCommandHandler : IRequestHandler<DeleteImSafeCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteImSafeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteImSafeCommand request, CancellationToken cancellationToken)
    {
        var assessment = await _context.ImSafeAssessments.FindAsync(new object[] { request.Id }, cancellationToken);
        if (assessment == null) return false;
        
        _context.ImSafeAssessments.Remove(assessment);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
