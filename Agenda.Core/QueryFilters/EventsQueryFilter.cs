namespace Agenda.Core.QueryFilters;

public class EventsQueryFilter
{
    public int Id { get; set; }
    public int CreatedBy { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; }
    public string EventType { get; set; }
    public int Status { get; set; } = 1;
    public string? SearchCriteria { get; set; }
    public DateTime? FilterDate { get; set; }
    public List<int>? Participants { get; set; }
}