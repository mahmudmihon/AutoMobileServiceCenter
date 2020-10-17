using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASC.Web.DataAccess.Interfaces
{
    public interface IRepository<T>
    {
        Task AddAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> GetByCondition(string condition);
        T GetBySpecificCondition(string condition);
        void Update(T entity);
        void Delete(T entity);
    }
}
