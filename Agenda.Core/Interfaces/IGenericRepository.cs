using System.Linq.Expressions;
using Agenda.Core.Entities.Core.ResponseApi;

namespace Agenda.Core.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate);
    Task<ResponsePostDetail> InsertAsync(T entity, string process = "INSERT");
    Task<ResponsePostDetail> InsertAsync(List<T> entities, string process = "INSERT");
    ResponsePostDetail UpdateCustom(T entity, params Expression<Func<T, object>>[] includeProperties);
    Task<ResponsePostDetail> DeleteAsync(int id, string proceso = "DELETE");
}