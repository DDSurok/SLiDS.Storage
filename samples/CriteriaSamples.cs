﻿using SLiDS.Storage.Api.Criteria;
using SLiDS.Storage.Api.Criteria.SqlClient;
using System;

namespace SLiDS.Samples
{
    static class CriteriaSamples
    {
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
            Console.WriteLine($"Parameter [ Name: \"{fCr.Params[0].ParameterName}\", Type: {fCr.Params[0].DbType}, Value: {fCr.Params[0].Value} ]");
            // Parameter [ Name: "p0", Type: String, Value: "Jhon Smith" ]
        }
    }
}
