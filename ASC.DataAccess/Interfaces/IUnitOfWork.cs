using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASC.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Queue<Task<Action>> RollbackActions { get; set; }
        string ConnectionString { get; set; }
        IRepository<T> Repository<T>() where T : TableEntity;
        void CommitTransaction();
    }
}
