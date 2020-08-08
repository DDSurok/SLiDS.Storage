using System;

namespace SLiDS.Samples
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("CriteriaSamples.EmptyCriteriaSample();");
            CriteriaSamples.EmptyCriteriaSample();
            Console.WriteLine();
            Console.WriteLine("CriteriaSamples.OrCriteriaSample();");
            CriteriaSamples.OrCriteriaSample();
            Console.WriteLine();
            Console.WriteLine("CriteriaSamples.AndCriteriaSample();");
            CriteriaSamples.AndCriteriaSample();
            Console.WriteLine();
            Console.WriteLine("CriteriaSamples.EqCriteriaSample();");
            CriteriaSamples.EqCriteriaSample();
            Console.WriteLine();
            Console.WriteLine("CriteriaSamples.LtCriteriaSample();");
            CriteriaSamples.LtCriteriaSample();
            Console.WriteLine();
            Console.WriteLine("CriteriaSamples.GtCriteriaSample();");
            CriteriaSamples.GtCriteriaSample();
            Console.ReadKey(false);
        }
    }
}
