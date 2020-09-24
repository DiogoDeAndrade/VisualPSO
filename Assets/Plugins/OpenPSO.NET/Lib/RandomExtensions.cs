using System;

namespace OpenPSO.Lib
{
    public static class RandomExtensions
    {
        public static double NextDouble(this Random rnd, double min, double max)
        {
            if (max <= min)
                throw new ArgumentException("min must be smaller than max");

            return rnd.NextDouble() * Math.Abs(max - min) + min;
        }
    }
}
