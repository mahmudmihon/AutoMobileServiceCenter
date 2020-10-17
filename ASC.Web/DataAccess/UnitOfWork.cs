using ASC.Web.Data;
using ASC.Web.DataAccess.Interfaces;
using System.Threading.Tasks;

namespace ASC.Web.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
