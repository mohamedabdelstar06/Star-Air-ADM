namespace StarAirAdm.Application.Features.Kneeboard.Commands;

public record DeleteKneeboardNoteCommand(int Id, string PilotId) : IRequest<bool>;

public class DeleteKneeboardNoteCommandHandler : IRequestHandler<DeleteKneeboardNoteCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteKneeboardNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteKneeboardNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _context.KneeboardNotes.FirstOrDefaultAsync(n => n.Id == request.Id && n.PilotId == request.PilotId, cancellationToken);
        if (note == null) return false;

        _context.KneeboardNotes.Remove(note);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
