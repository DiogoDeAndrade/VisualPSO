using OpenPSO.Lib;
using System.Collections.Generic;

namespace OpenPSO.Functions
{
    public class Quadric : IFunction
    {
        public double Evaluate(IList<double> position)
        {
            double fitAux, fitness = 0.0;

            for (int i = 0; i < position.Count; i++)
            {
                fitAux = 0;
                for (int j = 0; j < i; j++)
                {
                    fitAux += position[j];
                }
                fitness += fitAux * fitAux;
            }
            return fitness;
        }
    }
}
