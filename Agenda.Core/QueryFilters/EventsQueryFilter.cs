namespace Agenda.Core.QueryFilters;

public class EventsQueryFilter
{
    public int Id { get; set; }
    public int CreatedBy { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StarDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; }
    public string EventType { get; set; }
}