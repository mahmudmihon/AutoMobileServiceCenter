using System.Threading.Tasks;

namespace ASC.Business.Interfaces
{
    public interface ILogDataOperations
    {
        Task CreateLogAsync(string category, string message);
        Task CreateExceptionLogAsync(string id, string message, string trace);
        Task CreateUserActivityAsync(string mail, string action);
    }
}
