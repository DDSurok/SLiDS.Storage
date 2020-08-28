using SLiDS.Storage.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLiDS.Samples.ObjectSamples
{
    class DbObject : IDbObject<int>
    {
        public string TableName => "t_temp";
        public string TableSchema => "dbo";
        public IEnumerable<string> TableScripts => new[] {
            $"create table {TableSchema}.{TableName} ( id int not null identity(1, 1), data nvarchar(100) not null )"
        };
        public IEnumerable<ColumnInfo> Columns => new[] {
            new ColumnInfo { CanIdentity = true, CanNullable = false, ColumnName = "id", PropertyName = "Id", Type = ColumnType.Int32 },
            new ColumnInfo { CanIdentity = false, CanNullable = false, ColumnName = "data", PropertyName = "Data", Type = ColumnType.String }
        };
        public ColumnInfo IdInfo => Columns.First(c => c.PropertyName == "Id");
        public int Id { get; set; }
        public string Data { get; set; }
    }
}
