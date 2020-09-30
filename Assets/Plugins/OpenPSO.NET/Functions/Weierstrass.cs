using OpenPSO.Lib;
using System;
using System.Collections.Generic;

namespace OpenPSO.Functions
{
    /// <summary>
    /// Weierstrass function.
    /// </summary>
    /// <remarks>
    /// Characteristics:
    ///
    /// * $d$-dimensional.
    /// * Search domain: $-0.5 \leq x_i \leq 0.5,\: \forall i=1,\dots,d$
    /// * Minimum: $f(0,\dots,0)=0$
    ///
    /// Optimization setup suggestions:
    ///
    /// * Initialization domain: $-0.5 \leq x_i \leq 0.2,\: \forall i=1,\dots,d$
    /// * Stop criterion: 0.01
    ///
    /// References:
    ///
    /// * https://peerj.com/articles/cs-202/
    /// </remarks>
    public class Weierstrass : IFunction
    {
        public double Evaluate(IList<double> position) => Function(position);

        public static double Function(IList<double> position)
        {
            int i, j;
            double res;
            double sum;
            double a, b;
            int k_max;

            a = 0.5;
            b = 3.0;
            k_max = 20;
            res = 0.0;

            for (i = 0; i < position.Count; ++i)
            {
                sum = 0.0;
                for (j = 0; j <= k_max; j++)
                    sum += Math.Pow(a, j) *
                        Math.Cos(2.0 * Math.PI * Math.Pow(b, j) *
                        (position[i] + 0.5));
                res += sum;
            }

            sum = 0.0;
            for (j = 0; j <= k_max; ++j)
            {
                sum += Math.Pow(a, j)
                    * Math.Cos(2.0 * Math.PI * Math.Pow(b, j) * 0.5);
            }
            return res - position.Count * sum;
        }
    }
}
