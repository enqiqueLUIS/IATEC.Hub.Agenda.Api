using Microsoft.EntityFrameworkCore;
using IATEC.Hub.Agenda.Api.Data;
using IATEC.Hub.Agenda.Api.DTOs;
using IATEC.Hub.Agenda.Api.Models;

namespace IATEC.Hub.Agenda.Api.Services;

public interface IEventService
{
    Task<List<EventDto>> GetAllAsync(EventFilterDto filter);
    Task<EventDto?> GetByIdAsync(int id);
    Task<(EventDto? Result, string? Error)> CreateAsync(CreateEventDto dto);
    Task<(EventDto? Result, string? Error)> UpdateAsync(int id, UpdateEventDto dto);
    Task<bool> DeleteAsync(int id);
    Task<(bool Success, string? Error)> ShareAsync(int id, ShareEventDto dto);
    Task<DashboardDto> GetDashboardAsync(string? user);
}

public class EventService : IEventService
{
    private readonly AgendaDbContext _db;

    public EventService(AgendaDbContext db)
    {
        _db = db;
    }

    public async Task<List<EventDto>> GetAllAsync(EventFilterDto filter)
    {
        var query = _db.Events.Include(e => e.Shares).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(e =>
                e.Title.ToLower().Contains(s) ||
                (e.Description != null && e.Description.ToLower().Contains(s)) ||
                e.Location.ToLower().Contains(s));
        }

        if (filter.From.HasValue)
            query = query.Where(e => e.EndDate >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(e => e.StartDate <= filter.To.Value);

        if (!string.IsNullOrWhiteSpace(filter.CreatedBy))
            query = query.Where(e => e.CreatedBy == filter.CreatedBy);

        if (filter.IsExclusive.HasValue)
            query = query.Where(e => e.IsExclusive == filter.IsExclusive.Value);

        var events = await query.OrderBy(e => e.StartDate).ToListAsync();
        return events.Select(MapToDto).ToList();
    }

    public async Task<EventDto?> GetByIdAsync(int id)
    {
        var ev = await _db.Events.Include(e => e.Shares).FirstOrDefaultAsync(e => e.Id == id);
        return ev is null ? null : MapToDto(ev);
    }

    public async Task<(EventDto? Result, string? Error)> CreateAsync(CreateEventDto dto)
    {
        if (dto.StartDate >= dto.EndDate)
            return (null, "StartDate must be before EndDate.");

        if (dto.IsExclusive)
        {
            var overlap = await HasOverlapAsync(dto.CreatedBy, dto.StartDate, dto.EndDate, excludeId: null);
            if (overlap)
                return (null, "An exclusive event overlaps with an existing exclusive event for this user.");
        }

        var ev = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Location = dto.Location,
            IsExclusive = dto.IsExclusive,
            CreatedBy = dto.CreatedBy,
            CreatedAt = DateTime.UtcNow
        };

        _db.Events.Add(ev);
        await _db.SaveChangesAsync();
        return (MapToDto(ev), null);
    }

    public async Task<(EventDto? Result, string? Error)> UpdateAsync(int id, UpdateEventDto dto)
    {
        var ev = await _db.Events.Include(e => e.Shares).FirstOrDefaultAsync(e => e.Id == id);
        if (ev is null)
            return (null, "Event not found.");

        if (dto.StartDate >= dto.EndDate)
            return (null, "StartDate must be before EndDate.");

        if (dto.IsExclusive)
        {
            var overlap = await HasOverlapAsync(ev.CreatedBy, dto.StartDate, dto.EndDate, excludeId: id);
            if (overlap)
                return (null, "An exclusive event overlaps with an existing exclusive event for this user.");
        }

        ev.Title = dto.Title;
        ev.Description = dto.Description;
        ev.StartDate = dto.StartDate;
        ev.EndDate = dto.EndDate;
        ev.Location = dto.Location;
        ev.IsExclusive = dto.IsExclusive;
        ev.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return (MapToDto(ev), null);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var ev = await _db.Events.FindAsync(id);
        if (ev is null) return false;

        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<(bool Success, string? Error)> ShareAsync(int id, ShareEventDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.SharedWith))
            return (false, "SharedWith is required.");

        var ev = await _db.Events.FindAsync(id);
        if (ev is null)
            return (false, "Event not found.");

        var alreadyShared = await _db.EventShares
            .AnyAsync(s => s.EventId == id && s.SharedWith == dto.SharedWith);

        if (alreadyShared)
            return (false, "Event already shared with this user.");

        _db.EventShares.Add(new EventShare
        {
            EventId = id,
            SharedWith = dto.SharedWith,
            SharedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<DashboardDto> GetDashboardAsync(string? user)
    {
        var now = DateTime.UtcNow;
        var query = _db.Events.Include(e => e.Shares).AsQueryable();

        if (!string.IsNullOrWhiteSpace(user))
        {
            query = query.Where(e =>
                e.CreatedBy == user ||
                e.Shares.Any(s => s.SharedWith == user));
        }

        var current = await query
            .Where(e => e.StartDate <= now && e.EndDate >= now)
            .OrderBy(e => e.StartDate)
            .ToListAsync();

        var upcoming = await query
            .Where(e => e.StartDate > now)
            .OrderBy(e => e.StartDate)
            .Take(10)
            .ToListAsync();

        return new DashboardDto
        {
            CurrentEvents = current.Select(MapToDto).ToList(),
            UpcomingEvents = upcoming.Select(MapToDto).ToList()
        };
    }

    private async Task<bool> HasOverlapAsync(string user, DateTime start, DateTime end, int? excludeId)
    {
        return await _db.Events.AnyAsync(e =>
            e.CreatedBy == user &&
            e.IsExclusive &&
            e.Id != excludeId &&
            e.StartDate < end &&
            e.EndDate > start);
    }

    private static EventDto MapToDto(Event ev) => new()
    {
        Id = ev.Id,
        Title = ev.Title,
        Description = ev.Description,
        StartDate = ev.StartDate,
        EndDate = ev.EndDate,
        Location = ev.Location,
        IsExclusive = ev.IsExclusive,
        CreatedBy = ev.CreatedBy,
        CreatedAt = ev.CreatedAt,
        UpdatedAt = ev.UpdatedAt,
        SharedWith = ev.Shares.Select(s => s.SharedWith).ToList()
    };
}
