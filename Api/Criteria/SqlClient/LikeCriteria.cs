using System.Data.SqlClient;

namespace SLiDS.Storage.Api.Criteria.SqlClient
{
    public class LikeCriteria<T> : AColumnCriteria, ICriteria
    {
        public T Criteria { get; private set; }
        public LikeCriteria(string columnName, T criteria) : base(columnName) => Criteria = criteria;
        public override FormatedCriteria Format(int paramNumber) => new FormatedCriteria
        {
            Query = $"{ColumnName} LIKE @p{paramNumber}",
            Params = new[] { new SqlParameter($"p{paramNumber}", ConvertType(typeof(T))) { Value = Criteria } }
        };
    }
}
