using System.Collections.Generic;

namespace OpenPSO.Lib
{
    public interface IFunction
    {
        double Evaluate(IList<double> position);
    }
}
