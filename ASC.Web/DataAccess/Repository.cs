using ASC.Web.Data;
using ASC.Web.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ASC.Web.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public IEnumerable<T> GetByCondition(string condition)
        {
            var results = _context.Set<T>() as IQueryable<T>;
            var r =  results.Where(condition).ToList();
            return r;
        }

        public T GetBySpecificCondition(string condition)
        {
            var results = _context.Set<T>() as IQueryable<T>;
            return results.Where(condition).SingleOrDefault();
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
