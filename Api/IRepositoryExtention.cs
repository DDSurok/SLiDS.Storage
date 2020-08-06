using SLiDS.Storage.Api;
using SLiDS.Storage.Api.Criteria.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Information.Web.Domain.Api
{
    public static class IRepositoryExtention
    {
        public static IEnumerable<TData> GetByIds<TData, TId>(this IDbRepository<TData, TId> repo, IEnumerable<TId> ids)
            where TData : IDbObject<TId>, new()
            => repo.GetFilteredData(new InCriteria<TId>(new TData().IdInfo.ColumnName, ids), new Dictionary<string, SortDirection>()).Data;
        public static async Task<IEnumerable<TData>> GetByIdsAsync<TData, TId>(this IAsyncDbRepository<TData, TId> repo, IEnumerable<TId> ids)
            where TData : IDbObject<TId>, new()
            => (await repo.GetFilteredDataAsync(new InCriteria<TId>(new TData().IdInfo.ColumnName, ids), new Dictionary<string, SortDirection>())).Data;
        public static TData GetById<TData, TId>(this IDbRepository<TData, TId> repo, TId id)
            where TData : IDbObject<TId>, new()
            => repo.GetFilteredData(new InCriteria<TId>(new TData().IdInfo.ColumnName, new[] { id }), new Dictionary<string, SortDirection>()).Data.FirstOrDefault();
        public static async Task<TData> GetByIdAsync<TData, TId>(this IAsyncDbRepository<TData, TId> repo, TId id)
            where TData : IDbObject<TId>, new()
            => (await repo.GetFilteredDataAsync(new InCriteria<TId>(new TData().IdInfo.ColumnName, new[] { id }), new Dictionary<string, SortDirection>())).Data.FirstOrDefault();
    }
}
