using System.Collections.Concurrent;
using Agenda.Core.Entities.Core;
using Agenda.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Agenda.Infrastructure.Context.SQLServer;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IDbContextTransaction _transaction;
        
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public IGenericRepository<Users> UsersRepository => Repository<Users>();
    public IGenericRepository<Event> EventsRepository => Repository<Event>();
    public IGenericRepository<EventParticipant> EventParticipantsRepository => Repository<EventParticipant>();
    public IGenericRepository<UserEvent> UserEventsRepository => Repository<UserEvent>();
    public IGenericRepository<EventInvitation> EventInvitationsRepository => Repository<EventInvitation>();

    public IGenericRepository<T> Repository<T>() where T : class
    {
        if (_repositories.TryGetValue(typeof(T), out var repo)) 
            return (IGenericRepository<T>)repo;

        var repositoryInstance = new GenericRepository<T>(_context);
        _repositories.TryAdd(typeof(T), repositoryInstance);
        return repositoryInstance;
    }

    public int SaveChanges() => _context.SaveChanges();
    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    public IDbContextTransaction BeginTransaction()
    {
        _transaction = _context.Database.BeginTransaction();
        return _transaction;
    }
    
    public void Commit() => _transaction?.Commit(); 
    public void Rollback() => _transaction?.Rollback();

    public void Dispose()
    {
        _transaction?.Dispose();
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction == null)
            _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
                _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
        
    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        var strategy = _context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await BeginTransactionAsync();
            try
            { 
                await action();
                await CommitTransactionAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        });
    }

    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        
        return await strategy.ExecuteAsync(async () =>
        {
            await BeginTransactionAsync();
            try
            {
                var result = await action();
                await CommitTransactionAsync();
                return result;
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
        });
    }
}