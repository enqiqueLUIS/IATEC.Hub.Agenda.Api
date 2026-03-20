using System.Linq.Expressions;
using Agenda.Core.Entities.Core.ResponseApi;

namespace Agenda.Core.Interfaces;

public interface IGenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        // Búsquedas
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        T GetByCustom(Func<IQueryable<T>, T> query);
        IEnumerable<T> GetAll();
        Task<List<T>> GetAllAsync();
        Task<IEnumerable<T>> GetByCustomQuery(Func<IQueryable<T>, IEnumerable<T>> query);
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query();

        // Inserción
        ResponsePostDetail Insert(T entity, string process = "INSERT");
        ResponsePostDetail Insert(List<T> entities, string process = "INSERT");
        Task<ResponsePostDetail> InsertAsync(T entity, string process = "INSERT");
        Task<ResponsePostDetail> InsertAsync(List<T> entities, string process = "INSERT");

        // Actualización
        ResponsePostDetail Update(T entity, string proceso = "UPDATE");
        ResponsePostDetail Update(List<T> entities, string proceso = "UPDATE");
        ResponsePostDetail UpdateCustom(T entity, params Expression<Func<T, object>>[] includeProperties);

        // Eliminación
        ResponsePostDetail Delete(T entity, string proceso = "DELETE");
        ResponsePostDetail Delete(List<T> entities, string proceso = "DELETE");
        Task<ResponsePostDetail> DeleteAsync(int id, string proceso = "DELETE");

        Task AddAsyncWithoutSave(T entity);
        Task AddRangeAsyncWithoutSave(List<T> entities);
        void UpdateWithoutSave(T entity);
        void UpdateRangeWithoutSave(List<T> entities);
        void RemoveWithoutSave(T entity);
        void RemoveRangeWithoutSave(List<T> entities);

    }
}