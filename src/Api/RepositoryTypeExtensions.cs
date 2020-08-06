using System;
using System.Data;

namespace SLiDS.Storage.Api
{
    public static class RepositoryTypeExtensions
    {
        public static SqlDbType ToSqlDbType(this ColumnType value) => value switch
        {
            ColumnType.Decimal => SqlDbType.Decimal,
            ColumnType.Byte => SqlDbType.TinyInt,
            ColumnType.Int16 => SqlDbType.SmallInt,
            ColumnType.Int32 => SqlDbType.Int,
            ColumnType.Int64 => SqlDbType.BigInt,
            ColumnType.Boolean => SqlDbType.Bit,
            ColumnType.String => SqlDbType.NVarChar,
            ColumnType.Float => SqlDbType.Real,
            ColumnType.Double => SqlDbType.Float,
            ColumnType.Guid => SqlDbType.UniqueIdentifier,
            ColumnType.DateTime => SqlDbType.DateTime,
            ColumnType.Variant => SqlDbType.Variant,
            _ => throw new NotImplementedException()
        };
    }
    public static class SqlDbTypeExtensions
    {
        public static ColumnType ToRepositoryType(this SqlDbType value) => value switch
        {
            SqlDbType.Decimal => ColumnType.Decimal,
            SqlDbType.TinyInt => ColumnType.Byte,
            SqlDbType.SmallInt => ColumnType.Int16,
            SqlDbType.Int => ColumnType.Int32,
            SqlDbType.BigInt => ColumnType.Int64,
            SqlDbType.Bit => ColumnType.Boolean,
            SqlDbType.Char => ColumnType.String,
            SqlDbType.NChar => ColumnType.String,
            SqlDbType.VarChar => ColumnType.String,
            SqlDbType.NVarChar => ColumnType.String,
            SqlDbType.Text => ColumnType.String,
            SqlDbType.NText => ColumnType.String,
            SqlDbType.Real => ColumnType.Float,
            SqlDbType.Float => ColumnType.Double,
            SqlDbType.UniqueIdentifier => ColumnType.Guid,
            SqlDbType.DateTime => ColumnType.DateTime,
            SqlDbType.DateTime2 => ColumnType.DateTime,
            SqlDbType.Timestamp => ColumnType.DateTime,
            SqlDbType.Date => ColumnType.DateTime,
            SqlDbType.Time => ColumnType.DateTime,
            SqlDbType.Variant => ColumnType.Variant,
            _ => throw new NotImplementedException()
        };
    }
    public static class TypeExtensions
    {
        public static ColumnType ToRepositoryType(this Type value)
        {
            return value switch
            {
                var x when x == typeof(bool) => ColumnType.Boolean,
                var x when x == typeof(byte) => ColumnType.Byte,
                var x when x == typeof(short) => ColumnType.Int16,
                var x when x == typeof(int) => ColumnType.Int32,
                var x when x == typeof(long) => ColumnType.Int64,
                var x when x == typeof(decimal) => ColumnType.Decimal,
                var x when x == typeof(float) => ColumnType.Float,
                var x when x == typeof(double) => ColumnType.Double,
                var x when x == typeof(string) => ColumnType.String,
                var x when x == typeof(DateTime) => ColumnType.DateTime,
                var x when x == typeof(Guid) => ColumnType.Guid,
                var x when x == typeof(object) => ColumnType.Variant,
                _ => throw new NotSupportedException()
            };
        }
    }
}