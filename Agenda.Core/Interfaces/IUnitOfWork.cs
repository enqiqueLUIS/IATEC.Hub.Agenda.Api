using Microsoft.EntityFrameworkCore.Storage;

namespace Agenda.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
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