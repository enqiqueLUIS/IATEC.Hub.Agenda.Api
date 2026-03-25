using Agenda.Core.Entities.Core;
using Microsoft.EntityFrameworkCore.Storage;

namespace Agenda.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IGenericRepository<Users> UsersRepository { get; }
    public IGenericRepository<Event> EventsRepository { get; }
    public IGenericRepository<EventParticipant> EventParticipantsRepository { get; }
    public IGenericRepository<UserEvent> UserEventsRepository { get; }
    public IGenericRepository<EventInvitation> EventInvitationsRepository { get; }
    public IGenericRepository<T> Repository<T>() where T : class;

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}