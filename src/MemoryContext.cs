using SLiDS.Storage.Api;
using SLiDS.Storage.Api.Criteria;
using SLiDS.Storage.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SLiDS.Storage
{
    public class MemoryContext<TData, TId>
        : ICachedRepository<TData, TId>, IRepository<TData, TId>
        where TData : IObject<TId>, new()
    {
        private readonly IDictionary<TId, TData> _store = new Dictionary<TId, TData>();
        public IEnumerable<TData> Entry => _store.Values;

        public FilteredData<TData> GetFilteredData(ICriteria baseCriteria, IDictionary<string, SortDirection> orderByExpression = null, ICriteria filterCriteria = null, int pageSize = -1, int pageNumber = 0)
        {
            throw new NotImplementedException();
        }
        public TData Insert(TData data)
        {
            if (_store.ContainsKey(data.Id))
                throw new InsertOrUpdateItemException<TData>(data, new Exception("Хранилище содержит запись с таким ключом."));
            _store.Add(data.Id, data);
            return data;
        }
        public TData Update(TData data)
        {
            if (!_store.ContainsKey(data.Id))
                throw new InsertOrUpdateItemException<TData>(data, new Exception("В хранилище отсутствует запись с таким ключом."));
            _store[data.Id] = data;
            return data;
        }
        public void Delete(TId id)
        {
            if (!_store.ContainsKey(id))
                throw new DeleteItemByIdException<TData, TId>(id, new Exception("В хранилище отсутствует запись с таким ключом."));
            _store.Remove(id);
        }
        public void InsertRange(IEnumerable<TData> datas)
        {
            foreach (TData data in datas)
            {
                Insert(data);
            }
        }
        public IQueryable<TData> AsQueryable() => _store.Values.AsQueryable();
    }
}
