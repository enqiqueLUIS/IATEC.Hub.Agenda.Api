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
    int SaveChanges();
    Task<int> SaveChangesAsync();

    IDbContextTransaction BeginTransaction();
    void Commit();
    void Rollback();

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
        
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    Task ExecuteInTransactionAsync(Func<Task> action);
}