using System.Data.SqlClient;

namespace SLiDS.Storage.Api.Criteria.SqlClient
{
    public class LikeCriteria : AColumnCriteria, ICriteria
    {
        public string Criteria { get; private set; }
        public LikeCriteria(string columnName, string criteria) : base(columnName) => Criteria = criteria;
        private LikeCriteria() : base("") { }
        public override FormatedCriteria Format(int paramNumber) => new FormatedCriteria
        {
            Query = $"{ColumnName} LIKE @p{paramNumber}",
            Params = new[] { new SqlParameter($"p{paramNumber}", ConvertType(typeof(string))) { Value = Criteria } }
        };
    }
}
