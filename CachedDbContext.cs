using SLiDS.Storage.Api;
using SLiDS.Storage.Api.Criteria;
using SLiDS.Storage.Exceptions;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace SLiDS.Storage
{
    public class CachedDbContext<TData, TId> : DbContext<TData, TId>, ICachedRepository<TData, TId>
        where TData : IDbObject<TId>, new()
    {
        private readonly HashSet<TData> _set = new HashSet<TData>();
        public CachedDbContext(string connectionString) : base(connectionString)
        {
            foreach (TData item in GetFilteredData(new EmptyCriteria()).Data)
            {
                _set.Add(item);
            }
        }
        public override TData Insert(TData data) => Insert(data, null);
        public override TData Insert(TData data, DbTransaction transaction)
        {
            if (!(transaction! is null))
            {
                throw new NotSupportedActionInTransactionException();
            }
            TData temp = base.Insert(data, transaction);
            _set.Add(temp);
            return temp;
        }
        public override TData Update(TData data) => Update(data, null);
        public override TData Update(TData data, DbTransaction transaction)
        {
            if (!(transaction! is null))
            {
                throw new NotSupportedActionInTransactionException();
            }
            TData temp = base.Update(data, transaction);
            _set.RemoveWhere(d => d.Id.Equals(data.Id));
            _set.Add(temp);
            return temp;
        }
        public override void Delete(TId id) => Delete(id, null);
        public override void Delete(TId id, DbTransaction transaction)
        {
            if (!(transaction! is null))
            {
                throw new NotSupportedActionInTransactionException();
            }
            base.Delete(id, transaction);
            _set.RemoveWhere(d => d.Id.Equals(id));
        }
        public override Task<TData> InsertAsync(TData data) => InsertAsync(data, null);
        public override async Task<TData> InsertAsync(TData data, DbTransaction transaction)
        {
            if (!(transaction! is null))
            {
                throw new NotSupportedActionInTransactionException();
            }
            TData temp = await base.InsertAsync(data, transaction);
            _set.Add(temp);
            return temp;
        }
        public override Task<TData> UpdateAsync(TData data) => UpdateAsync(data, null);
        public override async Task<TData> UpdateAsync(TData data, DbTransaction transaction)
        {
            if (!(transaction! is null))
            {
                throw new NotSupportedActionInTransactionException();
            }
            TData temp = await base.UpdateAsync(data, transaction);
            _set.RemoveWhere(d => d.Id.Equals(data.Id));
            _set.Add(temp);
            return temp;
        }
        public override Task DeleteAsync(TId id) => DeleteAsync(id, null);
        public override async Task DeleteAsync(TId id, DbTransaction transaction)
        {
            if (!(transaction! is null))
            {
                throw new NotSupportedActionInTransactionException();
            }
            await base.DeleteAsync(id, transaction);
            _set.RemoveWhere(d => d.Id.Equals(id));
        }
        public IEnumerable<TData> Entry => _set;
        public IQueryable<TData> AsQueryable() => _set.AsQueryable();
    }
}