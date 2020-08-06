using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SLiDS.Storage.Api.Criteria.SqlClient
{
    public class InCriteria<T> : AColumnCriteria, ICriteria
    {
        public IEnumerable<T> Criterias { get; private set; }
        public InCriteria(string columnName, IEnumerable<T> criterias) : base(columnName) => Criterias = criterias;
        private InCriteria() : base("") { }
        public override FormatedCriteria Format(int paramNumber) => new FormatedCriteria
        {
            Query = $"{ColumnName} IN ({Enumerable.Range(paramNumber, Criterias.Distinct().Count()).Select(i => $"@p{i}").Aggregate((cur, next) => cur + ", " + next)})",
            Params = Criterias.Distinct().Select((cr, i) => new SqlParameter($"p{paramNumber + i}", ConvertType(typeof(T))) { Value = cr }).ToArray()
        };
    }
}
