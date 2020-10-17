using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASC.DataAccess.Interfaces
{
    public interface IRepository<T> where T : TableEntity
    {
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T> FindAsync(string pKey, string rKey);
        Task<IEnumerable<T>> FindAllByPartitionKeyAsync(string pKey);
        Task<IEnumerable<T>> FindAllAsync();
        Task CreateTableAsync();
    }
}
