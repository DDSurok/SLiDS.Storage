using SLiDS.Storage.Api.Criteria;
using SLiDS.Storage.Api.Criteria.SqlClient;
using System;
using System.Data.Common;

namespace SLiDS.Samples
{
    static class CriteriaSamples
    {
        public static string ToFormatedString(this DbParameter param)
        {
            return $"Parameter [ Name: \"{param.ParameterName}\", Type: {param.DbType}, Value: {param.Value} ]";
        }

        public static void EmptyCriteriaSample()
        {
            ICriteria cr = new EmptyCriteria();
            // or ICriteria cr = ICriteria.Empty;
            FormatedCriteria fCr = cr.Format(0);
            Console.WriteLine(fCr.Query);
            Console.WriteLine(fCr.Params.Length);
        }
        public static void AndCriteriaSample()
        {
            ICriteria cr = new AndCriteria(new[] { ICriteria.Empty, ICriteria.Empty });
            FormatedCriteria fCr = cr.Format(0);
            Console.WriteLine(fCr.Query);
            Console.WriteLine(fCr.Params.Length);
        }
        public static void OrCriteriaSample()
        {
            ICriteria cr = new OrCriteria(new[] { ICriteria.Empty, ICriteria.Empty });
            FormatedCriteria fCr = cr.Format(0);
            Console.WriteLine(fCr.Query);
            Console.WriteLine(fCr.Params.Length);
        }
        public static void EqCriteriaSample()
        {
            ICriteria cr = new EqCriteria<string>("customer", "Jhon Smith");
            FormatedCriteria fCr = cr.Format(0);
            Console.WriteLine(fCr.Query);             // "customer = @p0"
            Console.WriteLine(fCr.Params.Length);     // "1"
            Console.WriteLine(fCr.Params[0].ToFormatedString());
            // Parameter [ Name: "p0", Type: String, Value: "Jhon Smith" ]
        }
        public static void LtCriteriaSample()
        {
            ICriteria cr = new LtCriteria<float>("sale", (float) 0.5);
            FormatedCriteria fCr = cr.Format(0);
            Console.WriteLine(fCr.Query);             // "sale < @p0"
            Console.WriteLine(fCr.Params.Length);     // "1"
            Console.WriteLine(fCr.Params[0].ToFormatedString());
            // Parameter [ Name: "p0", Type: Single, Value: 0,5 ]
        }
        public static void GtCriteriaSample()
        {
            ICriteria cr = new GtCriteria<float>("sale", 0.5f);
            FormatedCriteria fCr = cr.Format(0);
            Console.WriteLine(fCr.Query);             // "sale > @p0"
            Console.WriteLine(fCr.Params.Length);     // "1"
            Console.WriteLine(fCr.Params[0].ToFormatedString());
            // Parameter [ Name: "p0", Type: Single, Value: 0,5 ]
        }
    }
}
