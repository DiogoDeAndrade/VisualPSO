using OpenPSO.Lib;
using System.Collections.Generic;

namespace OpenPSO.Functions
{
    /// <summary>
    /// Sphere function.
    /// </summary>
    /// <remarks>
    /// Characteristics:
    ///
    /// * $d$-dimensional.
    /// * Search domain: $-100 \leq x_i \leq 100,\: \forall i=1,\dots,d$
    /// * Minimum: $f(0,\dots,0)=0$
    ///
    /// Optimization setup suggestions:
    ///
    /// * Initialization domain: $50 \leq x_i \leq 100,\: \forall i=1,\dots,d$
    /// * Stop criterion: 0.01
    ///
    /// References:
    ///
    /// * https://peerj.com/articles/cs-202/
    /// </remarks>
    public class Sphere : IFunction
    {
        public double Evaluate(IList<double> position) => Function(position);

        public static double Function(IList<double> position)
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
