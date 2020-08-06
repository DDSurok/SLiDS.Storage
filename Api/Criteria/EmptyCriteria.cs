using System.Data.Common;

namespace SLiDS.Storage.Api.Criteria
{
    public class EmptyCriteria : ICriteria
    {
        public FormatedCriteria Format(int paramNumber)
        {
            return new FormatedCriteria
            {
                Params = new DbParameter[0],
                Query = "1 = 1"
            };
        }
    }
}
