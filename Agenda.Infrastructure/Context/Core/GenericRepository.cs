using System.Linq.Expressions;
using Agenda.Core.Entities.Core.ResponseApi;
using Agenda.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Context.SQLServer;

public class GenericRepository<T> : IGenericRepository.IGenericRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public T GetById(int id) => _dbSet.Find(id);
    public async Task<T> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public T GetByCustom(Func<IQueryable<T>, T> query)
            => query(_dbSet.AsQueryable());

    public async Task<IEnumerable<T>> GetByCustomQuery(Func<IQueryable<T>, IEnumerable<T>> query) 
        => query(_dbSet.AsQueryable());

    public IEnumerable<T> GetAll() => _dbSet.ToList();
    public async Task<List<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate) 
        => _dbSet.Where(predicate).ToList();
    
    public IQueryable<T> Query() => _dbSet.AsNoTracking().AsQueryable();

    public ResponsePostDetail Insert(T entity, string process = "INSERT")
    {
        _dbSet.Add(entity);
        int affected = _context.SaveChanges();
           return new ResponsePostDetail { Process = process, AffectedRows = affected };
    }

    public ResponsePostDetail Insert(List<T> entities, string process = "INSERT")
    {
        _dbSet.AddRange(entities);
        int affected = _context.SaveChanges();
        return new ResponsePostDetail { Process = process, AffectedRows = affected };
    }

    public async Task<ResponsePostDetail> InsertAsync(T entity, string process = "INSERT")
    {
        await _dbSet.AddAsync(entity);
        int affected = await _context.SaveChangesAsync();

        int idGenerated = 0;

        var idProp = entity.GetType().GetProperties()
            .FirstOrDefault(p => p.Name.StartsWith("Id") &&
                                 (p.PropertyType == typeof(int) || p.PropertyType == typeof(string)));

        if (idProp != null)
        {
            var idValue = idProp.GetValue(entity);

            if (idValue is int intValue)
            {
                idGenerated = intValue;
            }
            else if (idValue is string strValue && int.TryParse(strValue, out int parsed))
            {
                idGenerated = parsed;
            }
        }

        return new ResponsePostDetail
        {
            Process = process,
            AffectedRows = affected,
            IdGenerated = idGenerated
        };
    }
    
    public async Task<ResponsePostDetail> InsertAsync(List<T> entities, string process = "INSERT")
    {
        await _dbSet.AddRangeAsync(entities);
        int affected = await _context.SaveChangesAsync();
        return new ResponsePostDetail { Process = process, AffectedRows = affected };
    }

    public ResponsePostDetail Update(T entity, string process = "UPDATE")
    {
        _dbSet.Update(entity);
        int affected = _context.SaveChanges();
        return new ResponsePostDetail { Process = process, AffectedRows = affected };
    }

    public ResponsePostDetail Update(List<T> entities, string process = "UPDATE")
    {
        _dbSet.UpdateRange(entities);
        int affected = _context.SaveChanges(); return new ResponsePostDetail { Process = process, AffectedRows = affected };
    }

    public ResponsePostDetail UpdateCustom(T entity, params Expression<Func<T, object>>[] includeProperties)
    {
        var set = _context.Set<T>();
        var entityType = _context.Model.FindEntityType(typeof(T))!;
        var key = entityType.FindPrimaryKey()!;
        
        var local = set.Local.FirstOrDefault(localEntity =>
        {
            var localEntry = _context.Entry(localEntity);
            var incomingEntry = _context.Entry(entity);
            return key.Properties.All(p =>
                Equals(localEntry.Property(p.Name).CurrentValue, 
                    incomingEntry.Property(p.Name).CurrentValue));
        });

        if (local != null)
            _context.Entry(local).State = EntityState.Detached;

        // --- 2) Adjuntar el stub y marcar solo las props indicadas ---
        var entry = set.Attach(entity);

        // Asegura estado Unchanged y marca modificadas solo las solicitadas
        entry.State = EntityState.Unchanged;
        foreach (var prop in includeProperties) 
            entry.Property(prop).IsModified = true;

        // --- 3) Guardar ---
        var affected = _context.SaveChanges();
        
        return new ResponsePostDetail
        {
            Process = "UPDATE CUSTOM",
            AffectedRows = affected
        };
    }

    public async Task<ResponsePostDetail> DeleteAsync(int id, string process = "DELETE")
    {
        var entity = await GetByIdAsync(id);
        _dbSet.Remove(entity);
        int affected = await _context.SaveChangesAsync();
        return new ResponsePostDetail { Process = process, AffectedRows = affected };
    }

    public ResponsePostDetail Delete(T entity, string process = "DELETE")
    {
        _dbSet.Remove(entity);
        int affected = _context.SaveChanges();
        return new ResponsePostDetail { Process = process, AffectedRows = affected };
    }

    public ResponsePostDetail Delete(List<T> entities, string process = "DELETE")
    {
        _dbSet.RemoveRange(entities);
        int affected = _context.SaveChanges();
        return new ResponsePostDetail { Process = process, AffectedRows = affected };
    }

    public Task AddAsyncWithoutSave(T entity)
    {
        return _dbSet.AddAsync(entity).AsTask();
    }
    public Task AddRangeAsyncWithoutSave(List<T> entities)
    { 
        _dbSet.AddRange(entities);
        return Task.CompletedTask;
    }
    public void UpdateWithoutSave(T entity)
    { 
        _dbSet.Update(entity);
    }
    public void UpdateRangeWithoutSave(List<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }
    public void RemoveWithoutSave(T entity)
    {
        _dbSet.Remove(entity);
    }
    public void RemoveRangeWithoutSave(List<T> entities)
    { 
        _dbSet.RemoveRange(entities);
    }
}