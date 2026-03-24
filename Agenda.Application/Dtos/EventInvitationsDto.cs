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
    public InvitationEventDto? EventInfo { get; set; }
    public InvitationUserDto? SentBy { get; set; }
}

public class InvitationEventDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class InvitationUserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}