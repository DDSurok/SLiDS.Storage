using System.Collections.Generic;
using System.Linq;

namespace SLiDS.Storage.Api
{
    public interface ICachedRepository<TData, TId> : IRepository<TData, TId> where TData : IObject<TId>
    {
        IQueryable<TData> AsQueryable();
        IEnumerable<TData> Entry { get; }
    }
}
