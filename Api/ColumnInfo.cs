namespace SLiDS.Storage.Api
{
    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public string PropertyName { get; set; }
        public ColumnType Type { get; set; }
        public bool CanNullable { get; set; } = false;
        public bool CanIdentity { get; set; } = false;
        public bool NeedWriteOnline { get; set; }
        public bool NeedWriteToExport { get; set; }
    }
}