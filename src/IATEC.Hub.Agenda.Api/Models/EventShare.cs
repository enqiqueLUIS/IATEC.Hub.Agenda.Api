namespace IATEC.Hub.Agenda.Api.Models;

public class EventShare
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public Event Event { get; set; } = null!;
    public string SharedWith { get; set; } = string.Empty;
    public DateTime SharedAt { get; set; } = DateTime.UtcNow;
}
