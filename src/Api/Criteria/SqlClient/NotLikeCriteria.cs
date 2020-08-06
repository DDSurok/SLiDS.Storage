using System.Data.SqlClient;

namespace SLiDS.Storage.Api.Criteria.SqlClient
{
    public class NotLikeCriteria<T> : AColumnCriteria, ICriteria
    {
        public T Criteria { get; private set; }
        public NotLikeCriteria(string columnName, T criteria) : base(columnName) => Criteria = criteria;
        public override FormatedCriteria Format(int paramNumber) => new FormatedCriteria
        {
            Query = $"{ColumnName} NOT LIKE @p{paramNumber}",
            Params = new[] { new SqlParameter($"p{paramNumber}", ConvertType(typeof(T))) { Value = Criteria } }
        };
    }
}
