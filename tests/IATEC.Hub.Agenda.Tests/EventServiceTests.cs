using Microsoft.EntityFrameworkCore;
using IATEC.Hub.Agenda.Api.Data;
using IATEC.Hub.Agenda.Api.DTOs;
using IATEC.Hub.Agenda.Api.Services;

namespace IATEC.Hub.Agenda.Tests;

public class EventServiceTests
{
    private static AgendaDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AgendaDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AgendaDbContext(options);
    }

    // ──────────────────────────────────
    // CREATE
    // ──────────────────────────────────
    [Fact]
    public async Task Create_ValidEvent_ReturnsCreatedDto()
    {
        using var db = CreateContext(nameof(Create_ValidEvent_ReturnsCreatedDto));
        var svc = new EventService(db);

        var dto = new CreateEventDto
        {
            Title = "Test Event",
            StartDate = DateTime.UtcNow.AddHours(1),
            EndDate = DateTime.UtcNow.AddHours(2),
            Location = "Sala A",
            IsExclusive = false,
            CreatedBy = "user1"
        };

        var (result, error) = await svc.CreateAsync(dto);

        Assert.Null(error);
        Assert.NotNull(result);
        Assert.Equal("Test Event", result.Title);
        Assert.Equal("user1", result.CreatedBy);
    }

    [Fact]
    public async Task Create_StartAfterEnd_ReturnsError()
    {
        using var db = CreateContext(nameof(Create_StartAfterEnd_ReturnsError));
        var svc = new EventService(db);

        var dto = new CreateEventDto
        {
            Title = "Bad Event",
            StartDate = DateTime.UtcNow.AddHours(3),
            EndDate = DateTime.UtcNow.AddHours(1),
            CreatedBy = "user1"
        };

        var (result, error) = await svc.CreateAsync(dto);

        Assert.Null(result);
        Assert.NotNull(error);
    }

    // ──────────────────────────────────
    // EXCLUSIVE OVERLAP VALIDATION
    // ──────────────────────────────────
    [Fact]
    public async Task Create_ExclusiveOverlap_ReturnsError()
    {
        using var db = CreateContext(nameof(Create_ExclusiveOverlap_ReturnsError));
        var svc = new EventService(db);
        var now = DateTime.UtcNow;

        // First exclusive event
        var first = new CreateEventDto
        {
            Title = "Exclusive 1",
            StartDate = now.AddHours(1),
            EndDate = now.AddHours(3),
            IsExclusive = true,
            CreatedBy = "user1"
        };
        await svc.CreateAsync(first);

        // Overlapping exclusive event
        var second = new CreateEventDto
        {
            Title = "Exclusive 2",
            StartDate = now.AddHours(2),
            EndDate = now.AddHours(4),
            IsExclusive = true,
            CreatedBy = "user1"
        };

        var (result, error) = await svc.CreateAsync(second);

        Assert.Null(result);
        Assert.NotNull(error);
        Assert.Contains("exclusive", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Create_ExclusiveNonOverlap_Succeeds()
    {
        using var db = CreateContext(nameof(Create_ExclusiveNonOverlap_Succeeds));
        var svc = new EventService(db);
        var now = DateTime.UtcNow;

        await svc.CreateAsync(new CreateEventDto
        {
            Title = "Exclusive 1",
            StartDate = now.AddHours(1),
            EndDate = now.AddHours(2),
            IsExclusive = true,
            CreatedBy = "user1"
        });

        var (result, error) = await svc.CreateAsync(new CreateEventDto
        {
            Title = "Exclusive 2",
            StartDate = now.AddHours(3),
            EndDate = now.AddHours(4),
            IsExclusive = true,
            CreatedBy = "user1"
        });

        Assert.Null(error);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Create_ExclusiveOverlapDifferentUsers_Succeeds()
    {
        using var db = CreateContext(nameof(Create_ExclusiveOverlapDifferentUsers_Succeeds));
        var svc = new EventService(db);
        var now = DateTime.UtcNow;

        await svc.CreateAsync(new CreateEventDto
        {
            Title = "User1 Event",
            StartDate = now.AddHours(1),
            EndDate = now.AddHours(3),
            IsExclusive = true,
            CreatedBy = "user1"
        });

        var (result, error) = await svc.CreateAsync(new CreateEventDto
        {
            Title = "User2 Event",
            StartDate = now.AddHours(1),
            EndDate = now.AddHours(3),
            IsExclusive = true,
            CreatedBy = "user2"
        });

        Assert.Null(error);
        Assert.NotNull(result);
    }

    // ──────────────────────────────────
    // UPDATE
    // ──────────────────────────────────
    [Fact]
    public async Task Update_ExistingEvent_UpdatesData()
    {
        using var db = CreateContext(nameof(Update_ExistingEvent_UpdatesData));
        var svc = new EventService(db);

        var (created, _) = await svc.CreateAsync(new CreateEventDto
        {
            Title = "Original",
            StartDate = DateTime.UtcNow.AddHours(1),
            EndDate = DateTime.UtcNow.AddHours(2),
            CreatedBy = "user1"
        });

        var (updated, error) = await svc.UpdateAsync(created!.Id, new UpdateEventDto
        {
            Title = "Updated",
            StartDate = DateTime.UtcNow.AddHours(1),
            EndDate = DateTime.UtcNow.AddHours(3),
            Location = "New Location",
            IsExclusive = false
        });

        Assert.Null(error);
        Assert.Equal("Updated", updated!.Title);
        Assert.Equal("New Location", updated.Location);
    }

    [Fact]
    public async Task Update_NonExistent_ReturnsError()
    {
        using var db = CreateContext(nameof(Update_NonExistent_ReturnsError));
        var svc = new EventService(db);

        var (result, error) = await svc.UpdateAsync(999, new UpdateEventDto
        {
            Title = "X",
            StartDate = DateTime.UtcNow.AddHours(1),
            EndDate = DateTime.UtcNow.AddHours(2)
        });

        Assert.Null(result);
        Assert.Equal("Event not found.", error);
    }

    // ──────────────────────────────────
    // DELETE
    // ──────────────────────────────────
    [Fact]
    public async Task Delete_ExistingEvent_ReturnsTrue()
    {
        using var db = CreateContext(nameof(Delete_ExistingEvent_ReturnsTrue));
        var svc = new EventService(db);

        var (created, _) = await svc.CreateAsync(new CreateEventDto
        {
            Title = "To Delete",
            StartDate = DateTime.UtcNow.AddHours(1),
            EndDate = DateTime.UtcNow.AddHours(2),
            CreatedBy = "user1"
        });

        var deleted = await svc.DeleteAsync(created!.Id);
        var found = await svc.GetByIdAsync(created.Id);

        Assert.True(deleted);
        Assert.Null(found);
    }

    [Fact]
    public async Task Delete_NonExistent_ReturnsFalse()
    {
        using var db = CreateContext(nameof(Delete_NonExistent_ReturnsFalse));
        var svc = new EventService(db);

        var result = await svc.DeleteAsync(999);
        Assert.False(result);
    }

    // ──────────────────────────────────
    // SHARE
    // ──────────────────────────────────
    [Fact]
    public async Task Share_ValidEvent_AddsShare()
    {
        using var db = CreateContext(nameof(Share_ValidEvent_AddsShare));
        var svc = new EventService(db);

        var (created, _) = await svc.CreateAsync(new CreateEventDto
        {
            Title = "Shared Event",
            StartDate = DateTime.UtcNow.AddHours(1),
            EndDate = DateTime.UtcNow.AddHours(2),
            CreatedBy = "user1"
        });

        var (success, error) = await svc.ShareAsync(created!.Id, new ShareEventDto { SharedWith = "user2" });

        Assert.True(success);
        Assert.Null(error);

        var ev = await svc.GetByIdAsync(created.Id);
        Assert.Contains("user2", ev!.SharedWith);
    }

    [Fact]
    public async Task Share_DuplicateShare_ReturnsError()
    {
        using var db = CreateContext(nameof(Share_DuplicateShare_ReturnsError));
        var svc = new EventService(db);

        var (created, _) = await svc.CreateAsync(new CreateEventDto
        {
            Title = "Event",
            StartDate = DateTime.UtcNow.AddHours(1),
            EndDate = DateTime.UtcNow.AddHours(2),
            CreatedBy = "user1"
        });

        await svc.ShareAsync(created!.Id, new ShareEventDto { SharedWith = "user2" });
        var (success, error) = await svc.ShareAsync(created.Id, new ShareEventDto { SharedWith = "user2" });

        Assert.False(success);
        Assert.NotNull(error);
    }

    // ──────────────────────────────────
    // FILTER / SEARCH
    // ──────────────────────────────────
    [Fact]
    public async Task GetAll_SearchByTitle_FiltersCorrectly()
    {
        using var db = CreateContext(nameof(GetAll_SearchByTitle_FiltersCorrectly));
        var svc = new EventService(db);
        var now = DateTime.UtcNow;

        await svc.CreateAsync(new CreateEventDto { Title = "Team Meeting", StartDate = now.AddHours(1), EndDate = now.AddHours(2), CreatedBy = "user1" });
        await svc.CreateAsync(new CreateEventDto { Title = "Birthday Party", StartDate = now.AddHours(3), EndDate = now.AddHours(5), CreatedBy = "user1" });

        var results = await svc.GetAllAsync(new EventFilterDto { Search = "meeting", CreatedBy = "user1" });

        Assert.Single(results);
        Assert.Equal("Team Meeting", results[0].Title);
    }

    // ──────────────────────────────────
    // DASHBOARD
    // ──────────────────────────────────
    [Fact]
    public async Task GetDashboard_ReturnsCurrentAndUpcoming()
    {
        using var db = CreateContext(nameof(GetDashboard_ReturnsCurrentAndUpcoming));
        var svc = new EventService(db);
        var now = DateTime.UtcNow;

        // In progress
        await svc.CreateAsync(new CreateEventDto
        {
            Title = "In Progress",
            StartDate = now.AddMinutes(-30),
            EndDate = now.AddMinutes(30),
            CreatedBy = "user1"
        });

        // Upcoming
        await svc.CreateAsync(new CreateEventDto
        {
            Title = "Upcoming",
            StartDate = now.AddHours(2),
            EndDate = now.AddHours(3),
            CreatedBy = "user1"
        });

        var dashboard = await svc.GetDashboardAsync("user1");

        Assert.Single(dashboard.CurrentEvents);
        Assert.Equal("In Progress", dashboard.CurrentEvents[0].Title);
        Assert.Single(dashboard.UpcomingEvents);
        Assert.Equal("Upcoming", dashboard.UpcomingEvents[0].Title);
    }
}
