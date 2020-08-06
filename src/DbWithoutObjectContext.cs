using SLiDS.Storage.Api;
using SLiDS.Storage.Api.Criteria;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SLiDS.Storage
{
    public class DbWithoutObjectContext
    {
        public string ConnectionString { get; private set; }
        public string TableSchema { get; private set; }
        public DbWithoutObjectContext(string connectionString, string tableSchema)
        {
            ConnectionString = connectionString;
            TableSchema = tableSchema;
        }
        public ColumnInfo[] GetTableInfo(string tableName) => GetTableInfoAsync(tableName).Result;

        public FilteredData<dynamic> GetFilteredData(string tableName,
                                                     ICriteria baseCriteria,
                                                     IDictionary<string, SortDirection> orderByExpression = null,
                                                     ICriteria filterCriteria = null,
                                                     int pageSize = -1,
                                                     int pageNumber = 0)
            => GetFilteredDataAsync(tableName, baseCriteria, orderByExpression, filterCriteria, pageSize, pageNumber).Result;
        public async Task<ColumnInfo[]> GetTableInfoAsync(string tableName)
        {
            using DbConnection connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            using DbCommand command = connection.CreateCommand();
            command.Transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadUncommitted);
            command.CommandText = $"select top 0 * from {TableSchema}.{tableName}";
            using DbDataReader dr = await command.ExecuteReaderAsync();
            ColumnInfo[] returned = dr.GetColumnSchema().Select(item => new ColumnInfo
                {
                    ColumnName = item.ColumnName,
                    PropertyName = item.ColumnName,
                    Type = item.DataType.ToRepositoryType(),
                    CanIdentity = item.AllowDBNull ?? false
                }).ToArray();
            await command.Transaction.CommitAsync();
            await connection.CloseAsync();
            return returned;
        }
        public async Task<FilteredData<dynamic>> GetFilteredDataAsync(string tableName,
                                                                      ICriteria baseCriteria,
                                                                      IDictionary<string, SortDirection> orderByExpression = null,
                                                                      ICriteria filterCriteria = null,
                                                                      int pageSize = -1,
                                                                      int pageNumber = 0)
        {
            IDictionary<string, SortDirection> orderBy = CheckOrderByExpressionOrDefault(orderByExpression);
            ColumnInfo[] columns = await GetTableInfoAsync(tableName);
            using DbConnection connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            DbCommand command = connection.CreateCommand();
            command.Transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadUncommitted);
            int paramNumber = 0;
            FormatedCriteria baseFormatedCriteria = baseCriteria.Format(0);
            paramNumber = baseFormatedCriteria.Params.Length;
            command.CommandText = $@"
select count(*)
  from {TableSchema}.{tableName}
 where {baseFormatedCriteria.Query}";
            command.Parameters.AddRange(baseFormatedCriteria.Params);
            FilteredData<dynamic> data = new FilteredData<dynamic>
            {
                TotalCount = int.Parse(command.ExecuteScalar().ToString())
            };
            List<dynamic> records = new List<dynamic>();
            FormatedCriteria filterFormatedCriteria = (filterCriteria ?? ICriteria.Empty).Format(paramNumber);
            command.CommandText = $@"
select {columns.Select(c => c.ColumnName).Aggregate((cur, next) => cur + ", " + next)}
  from {TableSchema}.{tableName}
 where {baseFormatedCriteria.Query} and {filterFormatedCriteria.Query}
 order by {orderBy.Select(ex => $"{ex.Key} {ex.Value}").Aggregate((cur, next) => cur + ", " + next)}";
            if (pageSize > 0)
            {
                command.CommandText += $@"
offset @offset ROWS
 fetch next @fetch rows only";
                command.Parameters.AddRange(new[]
                {
                    new SqlParameter("offset", SqlDbType.Int) { Value = pageSize * pageNumber},
                    new SqlParameter("fetch", SqlDbType.Int) { Value = pageSize}
                });
            }
            command.Parameters.AddRange(filterFormatedCriteria.Params);
            using (DbDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    Dictionary<string, object> record = new Dictionary<string, object>();
                    for (int i = 0; i < columns.Length; i++)
                    {
                        ColumnInfo column = columns[i];
                        record.Add(column.PropertyName, column.CanNullable && dr.IsDBNull(i) ? null : GetTypedValue(dr, 0, column.Type));
                    }
                    records.Add(new RepositoryDynamicObject(record));
                }
            }
            data.Data = records;
            data.FilteredCount = records.Count;
            command.Transaction.Commit();
            command.Dispose();
            connection.Close();
            return data;
        }
        /// <summary>
        /// Возвращает заданный тип из DataReader.
        /// </summary>
        /// <param name="dr">Источник данных</param>
        /// <param name="index">Номер запрашиваемого столбца</param>
        /// <param name="type">Тип ожидаемого значения</param>
        /// <returns></returns>
        private object GetTypedValue(DbDataReader dr, int index, ColumnType type)
        {
            return type switch
            {
                ColumnType.Decimal => dr.GetDecimal(index),
                ColumnType.Byte => dr.GetByte(index),
                ColumnType.Int16 => dr.GetInt16(index),
                ColumnType.Int32 => dr.GetInt32(index),
                ColumnType.Int64 => dr.GetInt64(index),
                ColumnType.Boolean => dr.GetBoolean(index),
                ColumnType.String => dr.GetString(index),
                ColumnType.Float => dr.GetFloat(index),
                ColumnType.Double => dr.GetDouble(index),
                ColumnType.Guid => dr.GetGuid(index),
                ColumnType.DateTime => dr.GetDateTime(index),
                ColumnType.Variant => dr.GetValue(index),
                _ => throw new NotSupportedException($"Тип {type} столбца в БД не поддерживается.")
            };
        }
        /// <summary>
        /// Проверка и если пусто - заполнение стандартными значениями условий сортировки.
        /// По умолчанию сортировка по первому столбцу.
        /// </summary>
        /// <param name="orderByExpression">Переданный как параметр список сортировки</param>
        /// <returns>Переданный как параметр список сортировки или стандартный список сортировки, если исходный пуст</returns>
        private IDictionary<string, SortDirection> CheckOrderByExpressionOrDefault(IDictionary<string, SortDirection> orderByExpression)
        {
            Dictionary<string, SortDirection> orderBy =
                new Dictionary<string, SortDirection>(orderByExpression ?? new Dictionary<string, SortDirection>());
            if (orderBy.Count == 0)
            {
                orderBy.Add("1", SortDirection.ASC);
            }
            return orderBy;
        }
    }
}
