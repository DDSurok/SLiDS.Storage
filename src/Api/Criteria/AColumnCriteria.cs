using System;
using System.Data;

namespace SLiDS.Storage.Api.Criteria
{
    public abstract class AColumnCriteria : ICriteria
    {
        public string ColumnName { get; private set; }
        protected AColumnCriteria(string columnName) => ColumnName = columnName;
        protected SqlDbType ConvertType(Type type)
        {
            return type == typeof(int) ? SqlDbType.Int
                 : type == typeof(string) ? SqlDbType.NVarChar
                 : type == typeof(DateTime) ? SqlDbType.DateTime
                 : type == typeof(Guid) ? SqlDbType.UniqueIdentifier
                 : throw new NotSupportedException($"Неподдерживаемый тип {type.FullName}.");
        }
        public abstract FormatedCriteria Format(int paramNumber);
    }
}
