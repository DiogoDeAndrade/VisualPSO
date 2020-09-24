using System.Collections.Generic;

namespace OpenPSO.Lib.Topologies
{
    public class MooreGridTopology : StaticGridTopology
    {
        private static (int x, int y)[] mooreNeighs =
            {(0, 0), (0, 1), (1, 1), (1, 0),
		     (1, -1), (0, -1), (-1, -1), (-1, 0), (-1, 1)};

        public MooreGridTopology(int xSize, int ySize) : base(xSize, ySize) { }
        public MooreGridTopology() {}

        protected override IEnumerable<(int x, int y)> GetRelativeNeighbors()
            => mooreNeighs;
    }
}