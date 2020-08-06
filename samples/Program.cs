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
            Console.ReadKey(false);
        }
    }
}
