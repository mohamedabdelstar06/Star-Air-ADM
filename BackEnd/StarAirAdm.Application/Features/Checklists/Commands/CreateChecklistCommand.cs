namespace StarAirAdm.Application.Features.Checklists.Commands;

public record CreateChecklistCommand(CreateChecklistDto Dto, string CreatedBy) : IRequest<ChecklistResponseDto?>;

public class CreateChecklistCommandHandler : IRequestHandler<CreateChecklistCommand, ChecklistResponseDto?>
{
    private readonly IApplicationDbContext _context;

    public CreateChecklistCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ChecklistResponseDto?> Handle(CreateChecklistCommand request, CancellationToken cancellationToken)
    {
        var checklist = new Checklist
        {
            Title = request.Dto.Title,
            Category = request.Dto.Category,
            CreatedBy = request.CreatedBy,
            Items = request.Dto.Items.Select(i => new ChecklistItem
            {
                Description = i.Description,
                SortOrder = i.SortOrder,
                IsCritical = i.IsCritical
            }).ToList()
        };

        _context.Checklists.Add(checklist);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Map(checklist);
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
