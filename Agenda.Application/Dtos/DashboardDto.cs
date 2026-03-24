namespace Agenda.Application.Dtos;

public class DashboardEventDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string EventType { get; set; }
    public string CreatorName { get; set; }
}