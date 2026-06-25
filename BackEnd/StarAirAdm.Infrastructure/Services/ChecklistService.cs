using Microsoft.EntityFrameworkCore;
using StarAirAdm.Application.DTOs.Checklist;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Infrastructure.Data;

namespace StarAirAdm.Infrastructure.Services;

public class ChecklistService : IChecklistService
{
    private readonly AppDbContext _db;
    public ChecklistService(AppDbContext db) => _db = db;

    public async Task<ChecklistResponseDto?> CreateAsync(CreateChecklistDto dto, string createdBy)
    {
        var checklist = new Checklist
        {
            Title = dto.Title,
            Category = dto.Category,
            CreatedBy = createdBy,
            Items = dto.Items.Select(i => new ChecklistItem
            {
                Description = i.Description,
                SortOrder = i.SortOrder,
                IsCritical = i.IsCritical
            }).ToList()
        };
        _db.Checklists.Add(checklist);
        await _db.SaveChangesAsync();
        return Map(checklist);
    }

    public async Task<IEnumerable<ChecklistResponseDto>> GetAllAsync()
    {
        var list = await _db.Checklists.Include(c => c.Items)
            .OrderBy(c => c.Category).AsNoTracking().ToListAsync();
        return list.Select(Map);
    }

    public async Task<ChecklistResponseDto?> GetByIdAsync(int id)
    {
        var c = await _db.Checklists.Include(x => x.Items)
            .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return c == null ? null : Map(c);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var c = await _db.Checklists.FindAsync(id);
        if (c == null) return false;
        _db.Checklists.Remove(c);
        await _db.SaveChangesAsync();
        return true;
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
