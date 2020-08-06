using System.Collections.Generic;

namespace SLiDS.Storage.Api
{
    public class FilteredData<TData>
    {
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
        public IEnumerable<TData> Data { get; set; }
    }
}