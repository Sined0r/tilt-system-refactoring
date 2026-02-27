using System;

namespace TiltSystem
{
    public class Platform
    {
        public Screw Screw1 { get; }
        public Screw Screw2 { get; }
        public Screw Screw3 { get; }

        private readonly double _radius;

        public Platform(Screw screw1, Screw screw2, Screw screw3, double radius)
        {
            Screw1 = screw1;
            Screw2 = screw2;
            Screw3 = screw3;
            _radius = radius;
        }

        public TiltResult CalculateTilt()
        {
            return TiltCalculator.Calculate(
                Screw1.Height,
                Screw2.Height,
                Screw3.Height,
                _radius
            );
        }
    }
}