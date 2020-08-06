using SLiDS.Storage.Api;
using SLiDS.Storage.Api.Criteria;
using SLiDS.Storage.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SLiDS.Storage
{
    public class DbContext<TData, TId> : IDbRepository<TData, TId>, IAsyncDbRepository<TData, TId>
        where TData : IDbObject<TId>, new()
    {
        protected readonly TData _defaultTData = new TData();
        public string ConnectionString { get; protected set; }
        public DbContext(string connectionString)
        {
            ConnectionString = connectionString;
            if (!string.IsNullOrWhiteSpace(_defaultTData.TableName))
            {
                using DbConnection con = new SqlConnection(ConnectionString);
                con.Open();
                DbTransaction tran = con.BeginTransaction(IsolationLevel.Serializable);
                DbCommand com = con.CreateCommand();
                com.CommandText = @"
IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'T_Version'))
BEGIN
    CREATE TABLE dbo.T_Version (
        TableName sysname NOT NULL UNIQUE,
        Version int NOT NULL
    )
END";
                com.Transaction = tran;
                com.ExecuteNonQuery();
                com.CommandText = @"
SELECT Version FROM dbo.T_Version WHERE TableName = @tableName";
                com.Parameters.Add(new SqlParameter("tableName", SqlDbType.NVarChar) { Value = _defaultTData.TableName });
                com.Parameters.Add(new SqlParameter("version", SqlDbType.Int) { Value = 0 });
                int version = 0;
                using (DbDataReader dr = com.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        version = dr.GetInt32(0);
                    }
                }
                foreach (string script in _defaultTData.TableScripts.Skip(version))
                {
                    com.CommandText = script;
                    com.ExecuteNonQuery();
                    com.CommandText = @"
UPDATE dbo.T_Version SET Version = Version + 1 WHERE TableName = @tableName
IF @@ROWCOUNT = 0
BEGIN
    INSERT INTO dbo.T_Version (Version, TableName) values (1, @tableName)
END";
                    version++;
                    com.Parameters["version"].Value = version;
                    com.ExecuteNonQuery();
                }
                tran.Commit();
                con.Close();
            }
        }
        public DbTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            DbConnection con = new SqlConnection(ConnectionString);
            con.Open();
            return con.BeginTransaction(isolationLevel);
        }
        public void CommitTransaction(DbTransaction transaction) => CommitTransactionAsync(transaction).Wait();
        public void RollbackTransaction(DbTransaction transaction) => RollbackTransactionAsync(transaction).Wait();
        public FilteredData<TData> GetFilteredData(ICriteria baseCriteria,
                                                   IDictionary<string, SortDirection> orderByExpression = null,
                                                   ICriteria filterCriteria = null,
                                                   int pageSize = -1,
                                                   int page = 0
            ) => GetFilteredDataAsync(baseCriteria, orderByExpression, filterCriteria, pageSize, page).Result;
        public virtual TData Insert(TData data, DbTransaction transaction) => InsertAsync(data, transaction).Result;
        public virtual TData Insert(TData data) => Insert(data, null);
        public virtual TData Update(TData data, DbTransaction transaction) => UpdateAsync(data, transaction).Result;
        public virtual TData Update(TData data) => Update(data, null);
        public virtual void Delete(TId id, DbTransaction transaction) => DeleteAsync(id, transaction).Wait();
        public virtual void Delete(TId id) => Delete(id, null);
        public async Task<DbTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            DbConnection con = new SqlConnection(ConnectionString);
            await con.OpenAsync();
            return await con.BeginTransactionAsync(isolationLevel);
        }
        public async Task CommitTransactionAsync(DbTransaction transaction)
        {
            DbConnection connection = transaction.Connection;
            await transaction.CommitAsync();
            await connection.CloseAsync();
            await transaction.DisposeAsync();
            await connection.DisposeAsync();
        }
        public async Task RollbackTransactionAsync(DbTransaction transaction)
        {
            DbConnection connection = transaction.Connection;
            await transaction.RollbackAsync();
            await connection.CloseAsync();
            await transaction.DisposeAsync();
            await connection.DisposeAsync();
        }
        public virtual async Task<TData> InsertAsync(TData data, DbTransaction transaction)
        {
            try
            {
                DbTransaction usingTran = transaction ?? await BeginTransactionAsync(IsolationLevel.Serializable);
                DbCommand command = usingTran.Connection.CreateCommand();
                command.Transaction = usingTran;
                command.CommandText = $@"
insert into {_defaultTData.TableSchema}.{_defaultTData.TableName}
       ({_defaultTData.Columns.Where(c => !(c.CanIdentity && c.PropertyName == "Id"))
                              .Select(c => c.ColumnName)
                              .Aggregate((cur, next) => cur + ", " + next)})
values ({_defaultTData.Columns.Where(c => !(c.CanIdentity && c.PropertyName == "Id"))
                              .Select((c, i) => $"@p{i}")
                              .Aggregate((cur, next) => cur + ", " + next)})";
                if (_defaultTData.Columns.Any(c => c.CanIdentity && c.PropertyName == "Id"))
                {
                    command.CommandText += @"
select @@identity";
                }
                int i = 0;
                foreach (var col in _defaultTData.Columns.Where(c => !c.CanIdentity))
                {
                    command.Parameters.Add(new SqlParameter($"@p{i}", col.Type)
                    {
                        Value = typeof(TData).GetProperty(col.PropertyName).GetValue(data) ?? DBNull.Value
                    });
                    ++i;
                }
                if (_defaultTData.Columns.Any(c => c.CanIdentity && c.PropertyName == "Id"))
                    if (data.Id is int)
                        typeof(TData).GetProperty("Id").SetValue(data, int.Parse((await command.ExecuteScalarAsync()).ToString()));
                    else if (data.Id is long)
                        typeof(TData).GetProperty("Id").SetValue(data, long.Parse((await command.ExecuteScalarAsync()).ToString()));
                    else
                        throw new NotSupportedException($"Тип данных {typeof(TId).FullName} не поддерживается в качестве Identity-столбца.");
                else
                    await command.ExecuteNonQueryAsync();
                if (transaction is null)
                {
                    await CommitTransactionAsync(usingTran);
                }
                await command.DisposeAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new InsertOrUpdateItemException<TData>(data, ex);
            }
        }
        public virtual Task<TData> InsertAsync(TData data) => InsertAsync(data, null);
        public virtual async Task<TData> UpdateAsync(TData data, DbTransaction transaction)
        {
            if (_defaultTData.IdInfo is null)
            {
                throw new NotSupportedException($@"В описании таблицы для сущности {typeof(TData).FullName} не задан ключевой столбец.
Ключевой столбец должен быть привязан к свойству Id. В отсутствии ключевого столбца возможны только операции SELECT и INSERT.");
            }
            try
            {
                DbTransaction usingTran = transaction ?? await BeginTransactionAsync(IsolationLevel.Serializable);
                DbCommand command = usingTran.Connection.CreateCommand();
                command.Transaction = usingTran;
                ColumnInfo[] columns = _defaultTData.Columns.Where(c => c.PropertyName != "Id").ToArray();
                command.CommandText = $@"
update {_defaultTData.TableSchema}.{_defaultTData.TableName}
   set {columns.Select((c, i) => $"{c.ColumnName} = @p{i + 1}").Aggregate((cur, next) => cur + ", " + next)}
 where {_defaultTData.IdInfo.ColumnName} = @p0";
                command.Parameters.Add(new SqlParameter($"@p0", _defaultTData.IdInfo.Type) { Value = data.Id });
                int i = 1;
                foreach (var col in columns)
                {
                    command.Parameters.Add(new SqlParameter($"@p{i}", col.Type)
                    {
                        Value = typeof(TData).GetProperty(col.PropertyName).GetValue(data) ?? DBNull.Value
                    });
                    ++i;
                }
                if (_defaultTData.Columns.Any(c => c.CanIdentity && c.PropertyName == "Id"))
                    if (data.Id is int)
                        typeof(TData).GetProperty("Id").SetValue(data, int.Parse((await command.ExecuteScalarAsync()).ToString()));
                    else if (data.Id is long)
                        typeof(TData).GetProperty("Id").SetValue(data, long.Parse((await command.ExecuteScalarAsync()).ToString()));
                    else
                        throw new NotSupportedException($"Тип данных {typeof(TId).FullName} не поддерживается в качестве Identity-столбца.");
                else
                    await command.ExecuteNonQueryAsync();
                if (transaction is null)
                {
                    await CommitTransactionAsync(usingTran);
                }
                await command.DisposeAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new InsertOrUpdateItemException<TData>(data, ex);
            }
        }
        public virtual Task<TData> UpdateAsync(TData data) => UpdateAsync(data, null);
        public virtual async Task DeleteAsync(TId id, DbTransaction transaction)
        {
            if (_defaultTData.IdInfo is null)
            {
                throw new NotSupportedException($@"В описании таблицы для сущности {typeof(TData).FullName} не задан ключевой столбец.
Ключевой столбец должен быть привязан к свойству Id. В отсутствии ключевого столбца возможны только операции SELECT и INSERT.");
            }
            try
            {
                DbTransaction usingTran = transaction ?? await BeginTransactionAsync(IsolationLevel.Serializable);
                DbCommand command = usingTran.Connection.CreateCommand();
                command.Transaction = usingTran;
                command.CommandText = $@"
delete
  from {_defaultTData.TableSchema}.{_defaultTData.TableName}
 where {_defaultTData.IdInfo.ColumnName} = @p0";
                command.Parameters.Add(new SqlParameter("p0", _defaultTData.IdInfo.Type) { Value = id });
                await command.ExecuteNonQueryAsync();
                if (transaction is null)
                {
                    await CommitTransactionAsync(usingTran);
                }
                await command.DisposeAsync();
            }
            catch (Exception ex)
            {
                throw new DeleteItemByIdException<TData, TId>(id, ex);
            }
        }
        public virtual Task DeleteAsync(TId id) => DeleteAsync(id, null);
        public async Task<FilteredData<TData>> GetFilteredDataAsync(ICriteria baseCriteria,
            IDictionary<string, SortDirection> orderByExpression = null,
            ICriteria filterCriteria = null,
            int pageSize = -1,
            int pageNumber = 0)
        {
            IDictionary<string, SortDirection> orderBy = CheckOrderByExpressionOrDefault(orderByExpression);
            ColumnInfo[] columns = _defaultTData.Columns.ToArray();
            using DbConnection connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            DbCommand command = connection.CreateCommand();
            command.Transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadUncommitted);
            int paramNumber = 0;
            FormatedCriteria baseFormatedCriteria = baseCriteria.Format(0);
            paramNumber = baseFormatedCriteria.Params.Length;
            string pk = _defaultTData.IdInfo is null ? "*" : _defaultTData.IdInfo.ColumnName;
            command.CommandText = $@"
select count({pk})
  from {_defaultTData.TableSchema}.{_defaultTData.TableName}
 where {baseFormatedCriteria.Query}";
            command.Parameters.AddRange(baseFormatedCriteria.Params);
            FilteredData<TData> data = new FilteredData<TData>
            {
                TotalCount = int.Parse(command.ExecuteScalar().ToString())
            };
            List<TData> records = new List<TData>();
            FormatedCriteria filterFormatedCriteria = (filterCriteria ?? ICriteria.Empty).Format(paramNumber);
            command.CommandText = $@"
select count({pk})
  from {_defaultTData.TableSchema}.{_defaultTData.TableName}
 where {baseFormatedCriteria.Query} and {filterFormatedCriteria.Query}";
            data.FilteredCount = int.Parse(command.ExecuteScalar().ToString());
            command.CommandText = $@"
select {columns.Select(c => c.ColumnName).Aggregate((cur, next) => cur + ", " + next)}
  from {_defaultTData.TableSchema}.{_defaultTData.TableName}
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
                    TData record = new TData();
                    for (int i = 0; i < columns.Length; i++)
                    {
                        ColumnInfo column = columns[i];
                        typeof(TData).GetProperty(column.PropertyName)
                                     .SetValue(record, column.CanNullable && dr.IsDBNull(i) ? null : GetTypedValue(dr, i, column.Type));
                    }
                    records.Add(record);
                }
            }
            data.Data = records;

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
        protected object GetTypedValue(DbDataReader dr, int index, ColumnType type)
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
        /// Для сущностей с ключевым столбцом (связано со свойством "Id") - по умолчанию сортировка по ключевому столбцу.
        /// Для сущностей без ключевого столбца - по умолчанию сортировка по первому столбцу.
        /// </summary>
        /// <param name="orderByExpression">Переданный как параметр список сортировки</param>
        /// <returns>Переданный как параметр список сортировки или стандартный список сортировки, если исходный пуст</returns>
        protected IDictionary<string, SortDirection> CheckOrderByExpressionOrDefault(IDictionary<string, SortDirection> orderByExpression)
        {
            Dictionary<string, SortDirection> orderBy =
                new Dictionary<string, SortDirection>(orderByExpression ?? new Dictionary<string, SortDirection>());
            if (orderBy.Count == 0)
            {
                if (_defaultTData.IdInfo is null)
                    orderBy.Add(_defaultTData.Columns.First().ColumnName, SortDirection.ASC);
                else
                    orderBy.Add(_defaultTData.IdInfo.ColumnName, SortDirection.ASC);
            }
            return orderBy;
        }
    }
}