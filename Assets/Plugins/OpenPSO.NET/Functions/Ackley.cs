using OpenPSO.Lib;
using System;
using System.Collections.Generic;

namespace OpenPSO.Functions
{
    /// <summary>
    /// Ackley function.
    /// </summary>
    /// <remarks>
    /// Characteristics:
    ///
    /// * $d$-dimensional.
    /// * Search domain: $-32768 \leq x_i \leq 32768,\: \forall i=1,\dots,d$
    /// * Minimum: $f(0,\dots,0)=0$
    ///
    /// Optimization setup suggestions:
    ///
    /// * Initialization domain: $2.56 \leq x_i \leq 5.12,\: \forall i=1,\dots,d$
    /// * Stop criterion: 0.01
    ///
    /// References:
    ///
    /// * https://www.sfu.ca/~ssurjano/ackley.html
    /// * https://en.wikipedia.org/wiki/Ackley_function
    /// * https://peerj.com/articles/cs-202/
    /// </remarks>
    public class Ackley : IFunction
    {
        public double Evaluate(IList<double> position) => Function(position);

        public static double Function(IList<double> position)
        {
            double fitness = 0.0, fitaux1 = 0.0, fitaux2 = 0.0;

            for (int j = 0; j < position.Count; j++)
            {
                fitaux1 += position[j] * position[j];
                fitaux2 += Math.Cos(2 * Math.PI * position[j]);
            }

            fitness = -20 * Math.Exp(-0.2 * Math.Sqrt(fitaux1 / position.Count))
                - Math.Exp(fitaux2 / position.Count) + 20 + Math.E;

            return fitness;
        }
    }
}
