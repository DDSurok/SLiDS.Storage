using System.Data.SqlClient;

namespace SLiDS.Storage.Api.Criteria.SqlClient
{
    public class LtCriteria<T> : AColumnCriteria, ICriteria
    {
        public T Criteria { get; private set; }
        public LtCriteria(string columnName, T criteria) : base(columnName) => Criteria = criteria;
        private LtCriteria() : base("") { }
        public override FormatedCriteria Format(int paramNumber) => new FormatedCriteria
        {
            Query = $"{ColumnName} < @p{paramNumber}",
            Params = new[] { new SqlParameter($"p{paramNumber}", ConvertType(typeof(T))) { Value = Criteria } }
        };
    }
}
