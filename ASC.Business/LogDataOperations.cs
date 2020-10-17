using ASC.Business.Interfaces;
using System;
using System.Threading.Tasks;

namespace ASC.Business
{
    public class LogDataOperations : ILogDataOperations
    {
        public async Task CreateExceptionLogAsync(string id, string message, string stacktrace)
        {
           throw new NotImplementedException();
        }

        public async Task CreateLogAsync(string category, string message)
        {
            throw new NotImplementedException();
        }

        public async Task CreateUserActivityAsync(string email, string action)
        {
            throw new NotImplementedException();
        }
    }
}
