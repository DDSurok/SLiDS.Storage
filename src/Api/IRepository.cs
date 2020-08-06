using SLiDS.Storage.Api.Criteria;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SLiDS.Storage.Api
{
    public interface IRepository<TData, TId> where TData : IObject<TId>
    {
        FilteredData<TData> GetFilteredData(ICriteria baseCriteria,
            IDictionary<string, SortDirection> orderByExpression = null,
            ICriteria filterCriteria = null,
            int pageSize = -1,
            int pageNumber = 0);
        TData Insert(TData data);
        TData Update(TData data);
        void Delete(TId id);
    }

    public interface IAsyncRepository<TData, TId> where TData : IObject<TId>
    {
        Task<FilteredData<TData>> GetFilteredDataAsync(ICriteria baseCriteria,
            IDictionary<string, SortDirection> orderByExpression = null,
            ICriteria filterCriteria = null,
            int pageSize = -1,
            int pageNumber = 0);
        Task<TData> InsertAsync(TData data);
        Task<TData> UpdateAsync(TData data);
        Task DeleteAsync(TId id);
    }
}
