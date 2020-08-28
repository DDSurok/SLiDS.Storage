using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace SLiDS.Storage.Api
{
    public interface IDbRepository<TData, TId> : IRepository<TData, TId> where TData : IDbObject<TId>
    {
        string ConnectionString { get; }
        DbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable);
        void CommitTransaction(DbTransaction transaction);
        void RollbackTransaction(DbTransaction transaction);
        TData Insert(TData data, DbTransaction transaction);
        TData Update(TData data, DbTransaction transaction);
        void Delete(TId id, DbTransaction transaction);
    }

    public interface IAsyncDbRepository<TData, TId> : IAsyncRepository<TData, TId> where TData : IDbObject<TId>
    {
        string ConnectionString { get; }
        Task<DbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Serializable);
        Task RollbackTransactionAsync(DbTransaction transaction);
        Task CommitTransactionAsync(DbTransaction transaction);
        Task<TData> InsertAsync(TData data, DbTransaction transaction);
        Task<TData> UpdateAsync(TData data, DbTransaction transaction);
        Task DeleteAsync(TId id, DbTransaction transaction);
    }
}
