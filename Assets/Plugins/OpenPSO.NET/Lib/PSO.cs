using System;
using System.Linq;
using System.Collections.ObjectModel;
using OpenPSO.Lib.Topologies;

namespace OpenPSO.Lib
{
    public class PSO
    {

        // //////////////////////////// //
        // Required configurable fields //
        // //////////////////////////// //
        public Func<PSO, bool> UpdateStrategy { get; }

        // Inertia weight
        public Func<PSO, double> W { get; }

        // Acceleration coefficients, used to tune the relative influence of
        // each term of the formula
        public Func<PSO, double> C1 { get; }
        public Func<PSO, double> C2 { get; }

        public Func<PSO, double> XMin { get; }
        public Func<PSO, double> XMax { get; }
        public Func<PSO, double> VMax { get; }

        public Func<Particle, int, double> GroupBestPosition { get; }

        public double InitXMin { get; }
        public double InitXMax { get; }

        public IFunction Function { get; }
        public int NDims { get; }

        public int MaxEvals { get; }

        public double Criteria { get; }

        public bool CritKeepGoing { get; }

        public ITopology Topology { get; }


        // /////////////////////////////////////// //
        // Private fields which change at run time //
        // /////////////////////////////////////// //

        // Best fitness, position and particle ID so far
        private (double fitness, double[] position, Particle particle) bestSoFar;

        // Best fitness and particle ID at each iteration
        private (double fitness, Particle particle) bestCurr;
        // Worst fitness and particle ID at each iteration
        private (double fitness, Particle particle) worstCurr;

        // Random number generator
        private Random rng;

        // ///////////// //
        // Plugin events //
        // ///////////// //

        public event Action<PSO> PostIteration;
        public event Action<PSO> PostUpdatePopData;

        // ////////////////////////////// //
        // Publicly accessible properties //
        // ////////////////////////////// //

        // Average fitness at each iteration
        public double AvgFitCurr { get; private set; }

        public (double fitness, ReadOnlyCollection<double> position) BestSoFar
            => (bestSoFar.fitness, Array.AsReadOnly(bestSoFar.position));

        public int TotalEvals { get; private set; }

        // ////////////// //
        // Methods / Code //
        // ////////////// //

        public PSO(
            Func<PSO, bool> updateStrategy,
            Func<PSO, double> w,
            Func<PSO, double> c1,
            Func<PSO, double> c2,
            Func<PSO, double> xMin,
            Func<PSO, double> xMax,
            Func<PSO, double> vMax,
            double initXMin,
            double initXMax,
            IFunction function,
            int nDims,
            int maxEvals,
            double criteria,
            bool critKeepGoing,
            ITopology topology,
            int? seed = null)
        {
            UpdateStrategy = updateStrategy;
            W = w;
            C1 = c1;
            C2 = c2;
            XMin = xMin;
            XMax = xMax;
            VMax = vMax;
            InitXMin = initXMin;
            InitXMax = initXMax;
            Function = function;
            NDims = nDims;
            MaxEvals = maxEvals;
            Criteria = criteria;
            CritKeepGoing = critKeepGoing;
            Topology = topology;


            // Initialize list of particles
            Particle[] particles = new Particle[topology.PopSize];

            double[] position = new double[NDims];
            double[] velocity = new double[NDims];

            // Determine strategy for group best position
            if (topology is GlobalTopology)
                GroupBestPosition = (p, i) => BestSoFar.position[i];
            else
                GroupBestPosition = (p, i) => p.NeighsBestPositionSoFar[i];

            TotalEvals = 0;
            rng = seed.HasValue ? new Random(seed.Value) : new Random();

            // Initialize individual particles
            for (int i = 0; i < topology.PopSize; i++)
            {
                double fitness;

                for (int j = 0; j < NDims; j++)
                {
                    // Initialize position for current variable of current particle
                    position[j] = rng.NextDouble(InitXMin, InitXMax); // TODO What if [xMin, xMax] is different for different dimensions?

                    // Initialize velocity for current variable of current particle
                    velocity[j] = rng.NextDouble(XMin(this), XMax(this))
                        * rng.NextDouble(-0.5, 0.5);
                }

                // Determine fitness for initial position
                fitness = Function.Evaluate(position); // TODO Doesn't this count for PSO.TotalEvals?

                // TODO In practice it should be the topology to control particle ID
                particles[i] = new Particle(i, fitness, position, velocity);

                // TODO Hooks such as watershed
            }

            // Initialize topology
            topology.Init(particles);

            // Initialize bestSoFar as first particle
            bestSoFar = (particles[0].Fitness,
                particles[0].Position.ToArray(), particles[0]);
        }

        /// <summary>
        /// Update population data. Let particles know about particle with best
        /// and worst fitness in the population and calculate the average
        /// fitness in the population.
        /// </summary>
        /// <remarks>
        /// Client code will not usually call this function directly, unless
        /// more control is desired in the way the PSO algorithm advances.
        /// </remarks>
        public void UpdatePopData()
        {
            double sumFitness = 0;
            Particle pBest = null;
            Particle pWorst = null;

            foreach (Particle p in Topology.Particles)
            {
                // Update worst in population
                if (p.Fitness > (pWorst?.Fitness ?? float.NegativeInfinity)) // TODO Improve this for seeking max instead of min
                    pWorst = p;

                // Update best in population
                if (p.Fitness < (pBest?.Fitness ?? float.PositiveInfinity)) // TODO Improve this for seeking max instead of min
                    pBest = p;

                // Ask particle to update knowledge of best fitnesses/positions
                // so far
                p.UpdateBestSoFar();

                // Update total fitness
                sumFitness += p.Fitness;
            }

            // Update worst fitness/particle for current iteration
            worstCurr = (pWorst.Fitness, pWorst);

            // Update best fitness/particle for current iteration
            bestCurr = (pBest.Fitness, pBest);

            // Updates best fitness/position so far in population (i.e. all
            // iterations so far)
            if (bestCurr.fitness < bestSoFar.fitness) // TODO Improve this for seeking max instead of min
            {
                bestSoFar.fitness = bestCurr.fitness;
                bestSoFar.position = bestCurr.particle.Position.ToArray();
                bestSoFar.particle = bestCurr.particle;
            }

            // Determine average fitness in the population
            AvgFitCurr = sumFitness / Topology.PopSize;

            // Call post-update population data events
            PostUpdatePopData?.Invoke(this);

        }

        /// <summary>
        /// Update position and velocity of a single particle.
        /// </summary>
        /// <param name="p">Particle to update.</param>
        public void UpdateParticle(Particle p)
        {
            for (int i = 0; i < NDims; i++)
            {
                // Update velocity
                p.Velocity[i] = W(this) * p.Velocity[i]
                    + C1(this) * rng.NextDouble() * (p.BestPositionSoFar[i] - p.Position[i])
                    + C2(this) * rng.NextDouble() * (GroupBestPosition(p, i) - p.Position[i]);

                // Keep velocity in bounds
                if (p.Velocity[i] > VMax(this)) p.Velocity[i] = VMax(this);
                else if (p.Velocity[i] < -VMax(this)) p.Velocity[i] = -VMax(this);

                // Update position
                p.Position[i] = p.Position[i] + p.Velocity[i];

                // Keep position in bounds, stop particle if necessary
                if (p.Position[i] > XMax(this))
                {
                    p.Position[i] = XMax(this);
                    p.Velocity[i] = 0;
                }
                else if (p.Position[i] < XMin(this))
                {
                    p.Position[i] = XMin(this);
                    p.Velocity[i] = 0;
                }
            }

            // Obtain particle fitness for new position
            p.Fitness = Function.Evaluate(p.Position);

            // TODO Post-evaluation hooks, e.g. watershed
        }

        /// <summary>
        /// Update position and velocity of all or some of the particles.
        /// </summary>
        /// <remarks>
        /// Client code will not usually call this function directly, unless
        /// more control is desired in the way the PSO algorithm advances.
        /// </remarks>
        public void UpdateParticles()
        {
            int evals = 0;

            // Cycle through particles
            foreach (Particle pCurr in Topology.Particles)
            {
                // TODO Update or not to update according to SS-PSO

                // Only update neighborhood information if topology not global
                if (!(Topology is GlobalTopology))
                {
                    // Cycle through neighbors
                    foreach (Particle pNeigh in Topology.GetNeighbors(pCurr))  // TODO Consider extra neighbors (Small World PSO)
                    {
                        // TODO If a neighbor particle is the worst particle, mark current particle for updating (SS-PSO only)

                        // Does the neighbor know of better fitness than current
                        // particle?
                        if (pNeigh.BestFitnessSoFar < pCurr.NeighsBestFitnessSoFar) // TODO Improve this for seeking max instead of min
                        {
                            // If so, current particle will get that knowledge also
                            pCurr.UpdateBestNeighbor(pNeigh);
                        }
                    }
                }
                else
                {
                    // TODO Hooks specific for global topology?
                }

                // Update current particle?
                if (UpdateStrategy(this)) // TODO Connect this with SS-PSO
                {
                    UpdateParticle(pCurr);
                    evals++;
                }

            }

            TotalEvals += evals;
        }

        /// <summary>
        /// Execute a complete PSO run.
        /// </summary>
        public void Run()
        {
            //int iterations = 0;
            int critEvals = 0; // TODO This should be instance variable
            TotalEvals = 0;

            // Keep going until maximum number of evaluations is reached
            do
            {
                // Update iteration count for current run
                //iterations++;

                // Let particles know about best and worst fitness and determine
                // average fitness
                UpdatePopData();

                // Update all particles
                UpdateParticles();

                // Call end-of-iteration events
                PostIteration?.Invoke(this);

                // Is the best so far below the stop criteria? If so did we
                // already saved the number of evaluations required to get below
                // the stop criteria?
                if (bestSoFar.fitness < Criteria && critEvals == 0) // TODO Improve this for seeking max instead of min
                {
                    // Keep the number of evaluations which attained the stop
                    // criteria
                    critEvals = TotalEvals;

                    // Stop current run if I'm not supposed to keep going
                    if (!CritKeepGoing) break;

                }

            } while (TotalEvals < MaxEvals);
        }

    }
}
