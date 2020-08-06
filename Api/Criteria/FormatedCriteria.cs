using System.Data.Common;

namespace SLiDS.Storage.Api.Criteria
{
    public class FormatedCriteria
    {
        public string Query { get; set; }
        public DbParameter[] Params { get; set; }
    }
}
