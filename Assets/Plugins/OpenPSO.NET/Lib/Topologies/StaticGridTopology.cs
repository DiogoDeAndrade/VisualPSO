using System;
using System.Linq;
using System.Collections.Generic;

namespace OpenPSO.Lib.Topologies
{
    public abstract class StaticGridTopology : ITopology
    {
        // TODO Probably topology properties will have to have an attribute
        // denoting they are a configurable parameter via reflection
        // TODO In private set, check if xSize/ySize > 0
        protected int XSize { get; private set; }
        protected int YSize { get; private set; }

        public int PopSize => XSize * YSize;

        public IEnumerable<Particle> Particles
        {
            get
            {
                for (int y = 0; y < YSize; y++)
                {
                    for (int x = 0; x < XSize; x++)
                    {
                        yield return particles[x, y];
                    }
                }
            }
        }

        private Particle[,] particles;

        public StaticGridTopology(int xSize, int ySize)
        {
            this.XSize = xSize;
            this.YSize = ySize;
        }

        public StaticGridTopology() {}

        protected Particle GetParticle(int x, int y)
        {
            if (particles is null)
                throw new InvalidOperationException("Topology not initialized");
            if (x >= XSize)
                throw new ArgumentOutOfRangeException(
                    $"Invalid grid position x: {x} >= {XSize}");
            if (y >= YSize)
                throw new ArgumentOutOfRangeException(
                    $"Invalid grid position y: {y} >= {YSize}");
            return particles[x, y];
        }

        public void Init(IEnumerable<Particle> particles)
        {
            int i = 0;
            if ((particles?.Count() ?? -1) != XSize * YSize)
                throw new ArgumentException("Invalid number of particles");

            this.particles = new Particle[XSize, YSize];

            foreach (Particle p in particles)
            {
                this.particles[i % XSize, i / XSize] = p;
                i++;
            }
        }

        public IEnumerable<Particle> GetNeighbors(Particle p)
        {
            if (p.id >= PopSize)
                throw new ArgumentOutOfRangeException("Invalid particle ID");
            int xPos = p.id % XSize;
            int yPos = p.id / XSize;

            foreach ((int x, int y) relNeigh in GetRelativeNeighbors())
            {
                int x = xPos + relNeigh.x;
                int y = yPos + relNeigh.y;

                if (x < 0) x = XSize + x;
                if (x >= XSize) x = XSize - x;
                if (y < 0) y = YSize + y;
                if (y >= YSize) y = YSize - y;

                yield return particles[x, y];
            }
        }

        protected abstract IEnumerable<(int x, int y)> GetRelativeNeighbors();

    }
}