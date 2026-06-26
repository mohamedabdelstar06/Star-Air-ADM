namespace StarAirAdm.Application.Features.Kneeboard.Commands;

public record CreateKneeboardNoteCommand(CreateKneeboardNoteDto Dto, string PilotId) : IRequest<KneeboardNoteResponseDto?>;

public class CreateKneeboardNoteCommandHandler : IRequestHandler<CreateKneeboardNoteCommand, KneeboardNoteResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public CreateKneeboardNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<KneeboardNoteResponseDto?> Handle(CreateKneeboardNoteCommand request, CancellationToken cancellationToken)
    {
        var note = new KneeboardNote
        {
            PilotId = request.PilotId,
            Title = request.Dto.Title,
            Content = request.Dto.Content,
            Tags = request.Dto.Tags,
            IsSynced = request.Dto.IsSynced
        };

        _context.KneeboardNotes.Add(note);
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
