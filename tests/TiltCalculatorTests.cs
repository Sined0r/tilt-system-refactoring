using System;
using TiltSystem;

namespace TiltSystem.Tests
{
    public class TiltCalculatorTests
    {
        public static void RunAll()
        {
            ZeroTilt_ShouldReturnZero();
            NonSymmetricScrews_ShouldProduceTilt();
        }

        private static void ZeroTilt_ShouldReturnZero()
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

            Console.WriteLine("Zero tilt test passed.");
        }

        private static void NonSymmetricScrews_ShouldProduceTilt()
        {
            var s1 = new Screw(0.001, -0.005, 0.005);
            var s2 = new Screw(0.003, -0.005, 0.005);
            var s3 = new Screw(0.001, -0.005, 0.005);

            var platform = new Platform(s1, s2, s3, 0.1);

            var result = platform.CalculateTilt();

            if (Math.Abs(result.TiltX) < 0.00001)
            {
                throw new Exception("Test failed: expected non-zero TiltX.");
            }

            Console.WriteLine("Non-symmetric tilt test passed.");
        }
    }
}