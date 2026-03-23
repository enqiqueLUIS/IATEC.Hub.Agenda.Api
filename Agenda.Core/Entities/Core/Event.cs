namespace Agenda.Core.Entities.Core;

public class Event
{
    public int Id { get; set; }
    public int CreatedBy { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Location { get; set; }
    public string EventType { get; set; } // "Exclusive" | "Shared"
    public int Status { get; set; } = 1;
}