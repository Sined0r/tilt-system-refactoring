using System;

namespace TiltSystem
{
    public class Screw
    {
        private readonly double _minHeight;
        private readonly double _maxHeight;

        public double Height { get; private set; }

        public Screw(double initialHeight, double minHeight, double maxHeight)
        {
            _minHeight = minHeight;
            _maxHeight = maxHeight;
            SetHeight(initialHeight);
        }

        public void Adjust(double delta)
        {
            SetHeight(Height + delta);
        }

        private void SetHeight(double value)
        {
            Height = Math.Clamp(value, _minHeight, _maxHeight);
        }
    }
}