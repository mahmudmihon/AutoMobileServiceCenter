using System.Threading.Tasks;

namespace ASC.Web.DataAccess.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveAsync();
    }
}
