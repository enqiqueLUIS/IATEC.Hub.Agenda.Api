namespace Agenda.Application.Dtos;

public class EventParticipantsDto
{
    public int Id      { get; set; }
    public int EventId { get; set; }
    public int UserId  { get; set; }
}