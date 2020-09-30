using System.Collections.Generic;

namespace OpenPSO.Lib.Topologies
{
    public class VonNeumannGridTopology : StaticGridTopology
    {
        private static (int x, int y)[] vnNeighs =
            {(0, 0), (0, 1), (1, 0), (0, -1), (-1, 0)};

        public VonNeumannGridTopology(int xSize, int ySize)
            : base(xSize, ySize) { }
        public VonNeumannGridTopology() {}

        protected override IEnumerable<(int x, int y)> GetRelativeNeighbors()
            => vnNeighs;

    }
}