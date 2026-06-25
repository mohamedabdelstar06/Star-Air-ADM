using Microsoft.EntityFrameworkCore;
using StarAirAdm.Application.DTOs.Kneeboard;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Domain.Entities;
using StarAirAdm.Infrastructure.Data;

namespace StarAirAdm.Infrastructure.Services;

public class KneeboardService : IKneeboardService
{
    private readonly AppDbContext _db;
    public KneeboardService(AppDbContext db) => _db = db;

    public async Task<KneeboardNoteResponseDto?> CreateAsync(CreateKneeboardNoteDto dto, string pilotId)
    {
        var note = new KneeboardNote
        {
            PilotId = pilotId,
            Title = dto.Title,
            Content = dto.Content,
            Tags = dto.Tags,
            IsSynced = dto.IsSynced
        };
        _db.KneeboardNotes.Add(note);
        await _db.SaveChangesAsync();
        return Map(note);
    }

    public async Task<IEnumerable<KneeboardNoteResponseDto>> GetByPilotAsync(string pilotId)
    {
        var list = await _db.KneeboardNotes
            .Where(n => n.PilotId == pilotId)
            .OrderByDescending(n => n.UpdatedAt ?? n.CreatedAt)
            .AsNoTracking().ToListAsync();
        return list.Select(Map);
    }

    public async Task<KneeboardNoteResponseDto?> UpdateAsync(int id, CreateKneeboardNoteDto dto, string pilotId)
    {
        var note = await _db.KneeboardNotes.FirstOrDefaultAsync(n => n.Id == id && n.PilotId == pilotId);
        if (note == null) return null;
        note.Title = dto.Title;
        note.Content = dto.Content;
        note.Tags = dto.Tags;
        note.IsSynced = dto.IsSynced;
        note.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Map(note);
    }

    public async Task<bool> DeleteAsync(int id, string pilotId)
    {
        var note = await _db.KneeboardNotes.FirstOrDefaultAsync(n => n.Id == id && n.PilotId == pilotId);
        if (note == null) return false;
        _db.KneeboardNotes.Remove(note);
        await _db.SaveChangesAsync();
        return true;
    }

    private static KneeboardNoteResponseDto Map(KneeboardNote n) => new()
    {
        Id = n.Id, PilotId = n.PilotId, Title = n.Title,
        Content = n.Content, Tags = n.Tags, IsSynced = n.IsSynced,
        CreatedAt = n.CreatedAt, UpdatedAt = n.UpdatedAt
    };
}
