using System;
using System.Linq;
using System.Collections.Generic;

namespace OpenPSO.Lib.Topologies
{
    public class StaticRingTopology : ITopology
    {
        // TODO Probably topology properties will have to have an attribute
        // denoting they are a configurable parameter via reflection
        // TODO In private set, check if size > 0
        public int PopSize { get; private set; }
        public int Radius { get; private set; }

        public IEnumerable<Particle> Particles => particles;

        private Particle[] particles;

        public StaticRingTopology(int popSize, int radius)
        {
            PopSize = popSize;
            Radius = radius;
        }

        public StaticRingTopology() {}

        public void Init(IEnumerable<Particle> particles)
        {
            if ((particles?.Count() ?? -1) != PopSize)
                throw new ArgumentException("Invalid number of particles");
            if (Radius * 2 + 1 > PopSize)
                throw new InvalidOperationException(
                    $"Radius of {Radius} requires population of at least " +
                    $"{Radius * 2 + 1} particles, but population is {PopSize}");

            this.particles = particles.ToArray();
        }

        public IEnumerable<Particle> GetNeighbors(Particle p)
        {
            if (p.id >= PopSize)
                throw new ArgumentOutOfRangeException("Invalid particle ID");

            for (int i = p.id - Radius; i <= p.id + Radius; i++)
                yield return particles[RingIndex(i)];
        }

        private int RingIndex(int i)
        {
            if (i < 0) i = PopSize + i;
            else if (i >= PopSize) i = i - PopSize;
            return i;
        }

    }
}
