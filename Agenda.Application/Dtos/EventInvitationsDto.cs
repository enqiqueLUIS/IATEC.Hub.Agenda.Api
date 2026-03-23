namespace Agenda.Application.Dtos;

public class EventInvitationsDto
{
    public int Id { get; set; }
    public int EventId { get; set; }
    public int InvitedByUserId { get; set; }
    public int InvitedUserId { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string Status { get; set; }
}