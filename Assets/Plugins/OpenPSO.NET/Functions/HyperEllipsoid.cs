using OpenPSO.Lib;
using System.Collections.Generic;

namespace OpenPSO.Functions
{
    public class HyperEllipsoid : IFunction
    {
        public double Evaluate(IList<double> position)
        {
            double fitness = 0.0;

            for (int i = 0; i < position.Count; i++)
            {
                fitness += i * position[i] * position[i];
            }
            return fitness;
        }
    }
}
