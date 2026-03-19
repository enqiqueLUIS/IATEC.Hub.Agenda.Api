namespace IATEC.Hub.Agenda.Api.DTOs;

public class CreateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool IsExclusive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class UpdateEventDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool IsExclusive { get; set; }
}

public class EventDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool IsExclusive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<string> SharedWith { get; set; } = new();
}

public class ShareEventDto
{
    public string SharedWith { get; set; } = string.Empty;
}

public class EventFilterDto
{
    public string? Search { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? CreatedBy { get; set; }
    public bool? IsExclusive { get; set; }
}

public class DashboardDto
{
    public List<EventDto> CurrentEvents { get; set; } = new();
    public List<EventDto> UpcomingEvents { get; set; } = new();
}
