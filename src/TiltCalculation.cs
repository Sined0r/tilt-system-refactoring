using System;

namespace TiltSystem
{
    public class TiltResult
    {
        public double TiltX { get; }
        public double TiltY { get; }

        public TiltResult(double tiltX, double tiltY)
        {
            TiltX = tiltX;
            TiltY = tiltY;
        }
    }

    public static class TiltCalculator
    {
        public static TiltResult Calculate(
            double screw1Height,
            double screw2Height,
            double screw3Height,
            double screwRadius)
        {
            double deltaX = (screw2Height - screw1Height) / (2 * screwRadius);
            double deltaY = (screw3Height - (screw1Height + screw2Height) / 2.0)
                            / (Math.Sqrt(3) * screwRadius);

            double tiltX = Math.Atan(deltaX) * (180.0 / Math.PI);
            double tiltY = Math.Atan(deltaY) * (180.0 / Math.PI);

            return new TiltResult(tiltX, tiltY);
        }
    }
}