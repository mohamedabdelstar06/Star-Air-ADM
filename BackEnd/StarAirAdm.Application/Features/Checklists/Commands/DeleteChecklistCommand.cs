namespace StarAirAdm.Application.Features.Checklists.Commands;

public record DeleteChecklistCommand(int Id) : IRequest<bool>;

public class DeleteChecklistCommandHandler : IRequestHandler<DeleteChecklistCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteChecklistCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteChecklistCommand request, CancellationToken cancellationToken)
    {
        var c = await _context.Checklists.FindAsync(new object[] { request.Id }, cancellationToken);
        if (c == null) return false;

        _context.Checklists.Remove(c);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
