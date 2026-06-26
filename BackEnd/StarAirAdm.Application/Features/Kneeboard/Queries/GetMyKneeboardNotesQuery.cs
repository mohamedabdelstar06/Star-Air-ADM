namespace StarAirAdm.Application.Features.Kneeboard.Queries;

public record GetMyKneeboardNotesQuery(string PilotId) : IRequest<IEnumerable<KneeboardNoteResponseDto>>;

public class GetMyKneeboardNotesQueryHandler : IRequestHandler<GetMyKneeboardNotesQuery, IEnumerable<KneeboardNoteResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMyKneeboardNotesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<KneeboardNoteResponseDto>> Handle(GetMyKneeboardNotesQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.KneeboardNotes
            .Where(n => n.PilotId == request.PilotId)
            .OrderByDescending(n => n.UpdatedAt ?? n.CreatedAt)
            .AsNoTracking().ToListAsync(cancellationToken);
            
        return list.Select(Map);
    }

    private static KneeboardNoteResponseDto Map(KneeboardNote n) => new()
    {
        Id = n.Id, PilotId = n.PilotId, Title = n.Title,
        Content = n.Content, Tags = n.Tags, IsSynced = n.IsSynced,
        CreatedAt = n.CreatedAt, UpdatedAt = n.UpdatedAt
    };
}
