using System.Collections.Generic;

namespace OpenPSO.Lib
{
    public interface ITopology
    {
        int PopSize { get; }
        IEnumerable<Particle> Particles { get; }

        void Init(IEnumerable<Particle> particles);
        IEnumerable<Particle> GetNeighbors(Particle p);


    }
}
