namespace Agenda.Application.Dtos;

public class UserEventsDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public string Status { get; set; } = "Active";
}