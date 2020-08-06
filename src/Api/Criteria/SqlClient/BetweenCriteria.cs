using System.Data.SqlClient;

namespace SLiDS.Storage.Api.Criteria.SqlClient
{
    class BetweenCriteria<T> : AColumnCriteria, ICriteria
    {
        private readonly T _minValue;
        private readonly T _maxValue;
        public BetweenCriteria(string columnName, T minValue, T maxValue) : base(columnName)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }
        private BetweenCriteria() : base("") { }
        public override FormatedCriteria Format(int paramNumber) => new FormatedCriteria
        {
            Query = $"{ColumnName} BETWEEN @p{paramNumber} AND @p{paramNumber+1}",
            Params = new[] {
                new SqlParameter($"p{paramNumber}", ConvertType(typeof(T))) { Value = _minValue },
                new SqlParameter($"p{paramNumber+1}", ConvertType(typeof(T))) { Value = _maxValue }
            }
        };
    }
}
