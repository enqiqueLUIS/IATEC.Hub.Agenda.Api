namespace Agenda.Core.Entities.Core;

public class UserEvent
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public string Status { get; set; } = "Active";
}