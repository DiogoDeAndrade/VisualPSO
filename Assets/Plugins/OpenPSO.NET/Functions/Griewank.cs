using System;
using System.Collections.Generic;
using OpenPSO.Lib;

namespace OpenPSO.Functions
{
    /// <summary>
    /// Griewank function.
    /// </summary>
    /// <remarks>
    /// Characteristics:
    ///
    /// * $d$-dimensional.
    /// * Search domain: $-600 \leq x_i \leq 600,\: \forall i=1,\dots,d$
    /// * Minimum: $f(0,\dots,0)=0$
    ///
    /// Optimization setup suggestions:
    ///
    /// * Initialization domain: $300 \leq x_i \leq 600,\: \forall i=1,\dots,d$
    /// * Stop criterion: 0.05
    ///
    /// References:
    ///
    /// * https://peerj.com/articles/cs-202/
    /// * https://www.sfu.ca/~ssurjano/griewank.html
    /// </remarks>
    public class Griewank : IFunction
    {
        public double Evaluate(IList<double> position)
        {
            double fit1 = 0.0;
            double fit2 = 1.0;

            for (int i = 0; i < position.Count; i++)
            {
                fit1 += position[i] * position[i];
                fit2 *= Math.Cos(position[i] / Math.Sqrt(i + 1.0));
            }
            return 1 + (fit1 / 4000.0) - fit2;
        }
    }
}
