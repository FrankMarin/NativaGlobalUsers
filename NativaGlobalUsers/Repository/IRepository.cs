using NativaGlobalUsers.Models;
using System.Linq.Expressions;

namespace NativaGlobalUsers.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null);

        Task Create(T entity);

        Task<T> Get(Expression<Func<T, bool>> filter = null, bool tracked = true);

        Task Remove(T entidad);

        Task<User> UpdateUser(User entity);

        Task Save();
    }
}
