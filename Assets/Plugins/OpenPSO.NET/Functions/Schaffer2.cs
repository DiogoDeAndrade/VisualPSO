using System;
using System.Collections.Generic;
using OpenPSO.Lib;

namespace OpenPSO.Functions
{
    /// <summary>
    /// Schaffer function N. 2.
    /// </summary>
    /// <remarks>
    /// Characteristics:
    ///
    /// * Two-dimensions only.
    /// * Search domain: $-100 \leq x,y \leq 100$
    /// * Minimum: $f(0,0)=0$
    ///
    /// Optimization setup suggestions:
    ///
    /// * Initialization domain: $15 \leq x, y \leq 30$
    /// * Stop criterion: 0.00001
    ///
    /// References:
    ///
    /// * https://peerj.com/articles/cs-202/
    /// * https://www.sfu.ca/~ssurjano/schaffer2.html
    /// * https://en.wikipedia.org/wiki/Test_functions_for_optimization
    /// </remarks>
    public class Schaffer2 : IFunction
    {
        public double Evaluate(IList<double> position)
        {
            if (position.Count != 2)
                throw new ArgumentException(
                    $"{nameof(Schaffer2)} function only works in 2D");

            double sSum = position[0] * position[0] + position[1] * position[1];
            double tmp1 = Math.Sin(Math.Sqrt(sSum));
            double tmp2 = 1 + 0.001 * sSum;

            return 0.5 + (tmp1 * tmp1 - 0.5) / (tmp2 * tmp2);
        }
    }
}
