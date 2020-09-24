using OpenPSO.Lib;
using System.Collections.Generic;

namespace OpenPSO.Functions
{
    public class Sphere : IFunction
    {
        public double Evaluate(IList<double> position)
        {
            double fitness = 0.0;

            for (int i = 0; i < position.Count; i++)
            {
                fitness += position[i] * position[i];
            }
            return fitness;
        }
    }
}
