using System.Collections.Generic;

namespace SLiDS.Storage.Api
{
    public interface IObject<TId>
    {
        TId Id { get; set; }
        ColumnInfo IdInfo { get; }
    }

    public interface IDbObject<TId> : IObject<TId>
    {
        string TableName { get; }
        string TableSchema { get; }
        IEnumerable<string> TableScripts { get; }
        IEnumerable<ColumnInfo> Columns { get; }
    }
}