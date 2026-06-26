namespace StarAirAdm.Application.Features.Kneeboard.Commands;

public record UpdateKneeboardNoteCommand(int Id, CreateKneeboardNoteDto Dto, string PilotId) : IRequest<KneeboardNoteResponseDto?>;

public class UpdateKneeboardNoteCommandHandler : IRequestHandler<UpdateKneeboardNoteCommand, KneeboardNoteResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public UpdateKneeboardNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<KneeboardNoteResponseDto?> Handle(UpdateKneeboardNoteCommand request, CancellationToken cancellationToken)
    {
        var note = await _context.KneeboardNotes.FirstOrDefaultAsync(n => n.Id == request.Id && n.PilotId == request.PilotId, cancellationToken);
        if (note == null) return null;

        note.Title = request.Dto.Title;
        note.Content = request.Dto.Content;
        note.Tags = request.Dto.Tags;
        note.IsSynced = request.Dto.IsSynced;
        note.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        
        return Map(note);
    }

    private static KneeboardNoteResponseDto Map(KneeboardNote n) => new()
    {
        Id = n.Id, PilotId = n.PilotId, Title = n.Title,
        Content = n.Content, Tags = n.Tags, IsSynced = n.IsSynced,
        CreatedAt = n.CreatedAt, UpdatedAt = n.UpdatedAt
    };
}
