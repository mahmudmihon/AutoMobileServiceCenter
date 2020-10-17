using ASC.DataAccess.Interfaces;
using ASC.Models.BaseTypes;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASC.DataAccess
{
    public class Repository<T> : IRepository<T> where T : TableEntity, new()
    {
        private readonly CloudStorageAccount storageAccount;
        private readonly CloudTableClient tableClient;
        private readonly CloudTable storageTable;

        public IUnitOfWork Scope { get; set; }

        public Repository(IUnitOfWork scope)
        {
            storageAccount = CloudStorageAccount.Parse(scope.ConnectionString);
            tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(typeof(T).Name);

            storageTable = table;
            Scope = scope;
        }

        private async Task<Action> CreateRollbackAction(TableOperation operation)
        {
            if (operation.OperationType == TableOperationType.Retrieve)
                return null;

            var tableEntity = operation.Entity;
            var cloudTable = storageTable;

            switch(operation.OperationType)
            {
                case TableOperationType.Insert:
                    return async () => await UndoInsertOperationAsync(cloudTable, tableEntity);

                case TableOperationType.Delete:
                    return async () => await UndoDeleteOperation(cloudTable, tableEntity);

                case TableOperationType.Replace:
                    var retrieveResult = await cloudTable.ExecuteAsync(TableOperation.Retrieve(tableEntity.PartitionKey, tableEntity.RowKey));
                    return async () => await UndoReplaceOperation(cloudTable, retrieveResult.Result as DynamicTableEntity, tableEntity);

                default:
                    throw new InvalidOperationException("The storage operation cannot be identified.");
            }
        }

        private async Task UndoInsertOperationAsync(CloudTable cloudTable, ITableEntity tableEntity)
        {
            var deleteOperation = TableOperation.Delete(tableEntity);
            await cloudTable.ExecuteAsync(deleteOperation);
        }

        private async Task UndoDeleteOperation(CloudTable cloudTable, ITableEntity tableEntity)
        {
            var entityToRestore = tableEntity as BaseEntity;
            entityToRestore.IsDeleted = false;

            var insertOperation = TableOperation.Replace(tableEntity);
            await cloudTable.ExecuteAsync(insertOperation);
        }

        private async Task UndoReplaceOperation(CloudTable cloudTable, ITableEntity orginalEntity, ITableEntity newEntity)
        {
            if(orginalEntity != null)
            {
                if (!string.IsNullOrEmpty(newEntity.ETag))
                    orginalEntity.ETag = newEntity.ETag;

                var replaceOperation = TableOperation.Replace(orginalEntity);
                await cloudTable.ExecuteAsync(replaceOperation);
            }
        }

        public async Task<T> AddAsync(T entity)
        {
            var entityToInsert = entity as BaseEntity;
            entityToInsert.CreatedDate = DateTime.UtcNow;
            entityToInsert.UpdatedDate = DateTime.UtcNow;

            TableOperation insertOperation = TableOperation.Insert(entity);
            var result = await ExecuteAsync(insertOperation);
            return result.Result as T;
        }

        public async Task CreateTableAsync()
        {
            CloudTable table = tableClient.GetTableReference(typeof(T).Name);
            await table.CreateIfNotExistsAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            var entityToDelete = entity as BaseEntity;
            entityToDelete.UpdatedDate = DateTime.UtcNow;
            entityToDelete.IsDeleted = true;

            TableOperation deleteOperation = TableOperation.Replace(entityToDelete);
            await ExecuteAsync(deleteOperation);
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            TableQuery<T> query = new TableQuery<T>();
            TableContinuationToken tableContinuationToken = null;
            var result = await storageTable.ExecuteQuerySegmentedAsync(query, tableContinuationToken);

            return result.Results as IEnumerable<T>;

        }

        public async Task<IEnumerable<T>> FindAllByPartitionKeyAsync(string pKey)
        {
            TableQuery<T> query = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, pKey));
            TableContinuationToken tableContinuationToken = null;
            var result = await storageTable.ExecuteQuerySegmentedAsync(query, tableContinuationToken);

            return result.Results as IEnumerable<T>;
        }

        public async Task<T> FindAsync(string pKey, string rKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<T>(pKey, rKey);
            var result = await storageTable.ExecuteAsync(retrieveOperation);

            return result.Result as T;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var entityToUpdate = entity as BaseEntity;
            entityToUpdate.UpdatedDate = DateTime.UtcNow;

            TableOperation updateOperation = TableOperation.Replace(entity);
            var result = await ExecuteAsync(updateOperation);
            return result.Result as T;
        }

        private async Task<TableResult> ExecuteAsync(TableOperation operation)
        {
            var rollbackAction = CreateRollbackAction(operation);
            var result = await storageTable.ExecuteAsync(operation);
            Scope.RollbackActions.Enqueue(rollbackAction);
            return result;
        }
    }
}
