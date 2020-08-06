using System.Collections.Generic;
using System.Linq;

namespace SLiDS.Storage.Api.Criteria
{
    public class OrCriteria : ICriteria
    {
        public IEnumerable<ICriteria> InnerCriterias { get; private set; }

        public OrCriteria(IEnumerable<ICriteria> innerCriterias) => InnerCriterias = innerCriterias;

        private OrCriteria() { }

        public FormatedCriteria Format(int paramNumber)
        {
            int localNumber = paramNumber;
            List<FormatedCriteria> inners = new List<FormatedCriteria>();
            foreach (var item in InnerCriterias)
            {
                inners.Add(item.Format(localNumber));
                localNumber += inners.Last().Params.Length;
            }
            return new FormatedCriteria
            {
                Query = $"( {inners.Select(i => i.Query).Aggregate((cur, next) => cur + " OR " + next)} )",
                Params = inners.SelectMany(i => i.Params).ToArray()
            };
        }
    }
}
