namespace StarAirAdm.Application.Features.Checklists.Queries;

public record GetChecklistByIdQuery(int Id) : IRequest<ChecklistResponseDto?>;

public class GetChecklistByIdQueryHandler : IRequestHandler<GetChecklistByIdQuery, ChecklistResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public GetChecklistByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ChecklistResponseDto?> Handle(GetChecklistByIdQuery request, CancellationToken cancellationToken)
    {
        var c = await _context.Checklists.Include(x => x.Items)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            
        return c == null ? null : Map(c);
    }

    private static ChecklistResponseDto Map(Checklist c) => new()
    {
        Id = c.Id, Title = c.Title, Category = c.Category,
        CreatedBy = c.CreatedBy, CreatedAt = c.CreatedAt,
        Items = c.Items.OrderBy(i => i.SortOrder).Select(i => new ChecklistItemResponseDto
        {
            Id = i.Id, Description = i.Description,
            SortOrder = i.SortOrder, IsCritical = i.IsCritical
        }).ToList()
    };
}
