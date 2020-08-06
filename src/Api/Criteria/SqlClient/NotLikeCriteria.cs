using System.Data.SqlClient;

namespace SLiDS.Storage.Api.Criteria.SqlClient
{
    public class NotLikeCriteria : AColumnCriteria, ICriteria
    {
        public string Criteria { get; private set; }
        public NotLikeCriteria(string columnName, string criteria) : base(columnName) => Criteria = criteria;
        private NotLikeCriteria() : base("") { }
        public override FormatedCriteria Format(int paramNumber) => new FormatedCriteria
        {
            Query = $"{ColumnName} NOT LIKE @p{paramNumber}",
            Params = new[] { new SqlParameter($"p{paramNumber}", ConvertType(typeof(string))) { Value = Criteria } }
        };
    }
}
