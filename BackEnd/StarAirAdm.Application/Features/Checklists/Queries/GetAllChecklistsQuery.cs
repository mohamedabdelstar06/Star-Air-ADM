namespace StarAirAdm.Application.Features.Checklists.Queries;

public record GetAllChecklistsQuery() : IRequest<IEnumerable<ChecklistResponseDto>>;

public class GetAllChecklistsQueryHandler : IRequestHandler<GetAllChecklistsQuery, IEnumerable<ChecklistResponseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllChecklistsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ChecklistResponseDto>> Handle(GetAllChecklistsQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.Checklists.Include(c => c.Items)
            .OrderBy(c => c.Category).AsNoTracking().ToListAsync(cancellationToken);
            
        return list.Select(Map);
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
