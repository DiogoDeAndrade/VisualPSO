using System;
using System.Linq;
using System.Collections.Generic;

namespace OpenPSO.Lib
{
    public class Particle
    {

        // Best position so far for this particle
        private readonly double[] bestPositionSoFar;

        private readonly double[] neighsBestPositionSoFar;

        private readonly double[] position;
        private readonly double[] velocity;

        public readonly int id;

        // Best position so far for this particle
        public IList<double> BestPositionSoFar =>  bestPositionSoFar;

        public IList<double> NeighsBestPositionSoFar => neighsBestPositionSoFar;

        public IList<double> Position => position;
        public IList<double> Velocity => velocity;

        // Current fitness
        public double Fitness { get; set; }

        // Best fitness this particle ever had so far
        public double BestFitnessSoFar { get; private set; }

        // Best fitness ever known by neighbors
        public double NeighsBestFitnessSoFar { get; private set; }


        public Particle(int id, double initFitness,
            IList<double> initPosition, IList<double> initVelocity)
        {
            this.id = id;
            Fitness = initFitness;

            position = initPosition.ToArray();
            velocity = initVelocity.ToArray();
            bestPositionSoFar = new double[position.Length];
            neighsBestPositionSoFar = new double[position.Length];

            // Set best position so far as current position
            Array.Copy(this.position, bestPositionSoFar, this.position.Length);

            // Set best neighbor position so far as myself
            Array.Copy(this.position, neighsBestPositionSoFar, this.position.Length);

            // Set my own fitness as best fitness so far
            BestFitnessSoFar = Fitness;

            // Set me as the best neighbor so far
            NeighsBestFitnessSoFar = Fitness;
        }

        public void UpdateBestNeighbor(Particle neighbor)
        {
            NeighsBestFitnessSoFar = neighbor.BestFitnessSoFar;
            Array.Copy(
                neighbor.bestPositionSoFar, // Source
                neighsBestPositionSoFar,    // Destination
                position.Length);
        }

        public void UpdateBestSoFar()
        {
            // Update knowledge of best fitness/position so far
            if (Fitness < BestFitnessSoFar) // TODO Improve this for seeking max instead of min
            {
                BestFitnessSoFar = Fitness;
                Array.Copy(position, bestPositionSoFar, position.Length);
            }

            // Update knowledge of best neighbor so far if I am the best neighbor
            if (Fitness < NeighsBestFitnessSoFar) // TODO Improve this for seeking max instead of min
            {
                NeighsBestFitnessSoFar = Fitness;
                Array.Copy(position, neighsBestPositionSoFar, position.Length);
            }
        }
    }
}
