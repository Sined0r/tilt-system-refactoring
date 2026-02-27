using System;
using TiltSystem;

namespace TiltSystem.Tests
{
    public class TiltCalculatorTests
    {
        public static void Run()
        {
            double h1 = 0.001;
            double h2 = 0.001;
            double h3 = 0.001;
            double radius = 0.1;

            var result = TiltCalculator.Calculate(h1, h2, h3, radius);

            if (Math.Abs(result.TiltX) > 0.00001 ||
                Math.Abs(result.TiltY) > 0.00001)
            {
                throw new Exception("Test failed: expected zero tilt.");
            }

            Console.WriteLine("Test passed.");
        }
    }
}