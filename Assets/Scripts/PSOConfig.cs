using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;
using System.IO;

public class PSOConfig : MonoBehaviour
{
    public enum Algorithm { PSO = 1, SSPSO = 2 };
    public enum Problem { Sphere = 1, Quadric = 2, Hyper = 3, Rastrigin = 4,
                          Griewank = 5, Schaffer = 6, Weierstrass = 7,
                          Ackley = 8, ShiftedQuadricWithNoise = 9, RotatedGriewank = 10,
                          PerlinLandscape = 56,
                          ImageSaturation = 57,
                          ImageValue = 58,
                          RalphBellCurveMean = 59, RalphBellCurveVariance = 60 };
    public enum InertiaWeight { Fixed = 0, TVIW = 1 }
    public enum CStrategy { Fixed = 0, TVACPSO = 1 }
    public enum WatershedStrategy { None, WorstSoFar, WorstLastIteration, BestWorst };
    public enum Topology { StaticRing1D, StaticGrid2D, StaticGraph, Global };
    public enum NeightborhoodType { Moore, VN, Ring };

    [Label("Use OpenPSO.NET"), HideInInspector]
    public bool                 useOpenPSONET = true;
    public int                  functionSamplingSize = 257;
    public int                  seed = 12345;
    [HideInInspector] public int                  maxIterations = 10000;
    [HideInInspector] public int                  maxEvaluations = 980000;
    public Algorithm            algorithm = Algorithm.PSO;
    public Problem              problem = Problem.PerlinLandscape;
    [ShowIf("IsPerlinLandscapeAndPSONET")]
    public int                  perlinOctaves = 8;
    [ShowIf("IsPerlinLandscapeAndPSONET")]
    public float                perlinAmplitude = 20;    
    [ShowIf("IsPerlinLandscapeAndPSONET")]
    public Vector2              perlinFrequency = new Vector2(0.02f, 0.02f);
    [ShowIf("IsPerlinLandscapeAndPSONET")]
    public float                perlinAmplitudePerOctave = 0.5f;
    [ShowIf("IsPerlinLandscapeAndPSONET")]
    public float                perlinFrequencyPerOctave = 2.0f;
    [ShowIf("IsPerlinLandscapeAndPSONET")]
    public Vector2              perlinOffset = Vector2.zero;
    [ShowIf("IsImageAndPSONET")]
    public Texture2D            image;
    [ShowIf("IsImageAndPSONET")]
    public bool                 invertImage = false;
    [ShowIf("IsRalph")]
    public int                  sampleRadius = 5;
    [ShowIf("IsRalph"),Label("Use Stimulus Instead of Response")]
    public bool                 useStimulus = false;
    public float                xMax = 100;
    public float                vMax = 1;
    [HideInInspector] public float                chi = 1;
    public float                omega = 0.729844f;
    public float                c1 = 1.5f;
    public float                c2 = 1.5f;
    [HideInInspector] public InertiaWeight        weightStrategy = 0;
    [HideInInspector] public CStrategy            cStrategy = 0;
    [HideInInspector] public bool                 assymetricInitialization = true;
    [ShowIf("assymetricInitialization")]
    public Vector2              initialX = new Vector2(-100.0f, 100.0f);
    [HideInInspector] public int                  numExtraRandomNeighbours = 0;
    [HideInInspector] public float                stopCriterion = 0.01f;
    [HideInInspector] public bool                 continueAfterStop = false;
    [HideInInspector] public WatershedStrategy    watershedStrategy = WatershedStrategy.None;
    public Topology             topology = Topology.StaticGrid2D;
    [ShowIf("IsStaticGrid2D")]
    public Vector2Int           dimension = new Vector2Int(7,7);
    [ShowIf("IsStaticGrid2D")]
    public NeightborhoodType    neighborhoodType = NeightborhoodType.VN;
    [ShowIf("IsStaticRing1DOrGlobal")]
    public int                  nParticles = 10;
    [ShowIf("IsStaticRing1D")]
    public float                radius = 1;
    [ShowIf("IsStaticGraph")]
    public string               tgf_filename = "graphs/ring_n10r1.tgf";

    System.Diagnostics.Process psoRun;

    bool IsStaticRing1DOrGlobal()
    {
        return topology == Topology.StaticRing1D || topology == Topology.Global;
    }

    bool IsStaticRing1D()
    {
        return topology == Topology.StaticRing1D;
    }

    bool IsStaticGrid2D()
    {
        return topology == Topology.StaticGrid2D;
    }

    bool IsStaticGraph()
    {
        return topology == Topology.StaticGraph;
    }

    bool IsPerlinLandscapeAndPSONET()
    {
        return (problem == Problem.PerlinLandscape) && (useOpenPSONET);
    }

    bool IsImageAndPSONET()
    {
        if (!useOpenPSONET) return false;

        return (problem == Problem.ImageValue) || (problem == Problem.ImageSaturation) || (problem == Problem.RalphBellCurveMean) || (problem == Problem.RalphBellCurveVariance);
    }

    bool IsRalph()
    {
        if (!IsImageAndPSONET()) return false;

        return (problem == Problem.RalphBellCurveMean) || (problem == Problem.RalphBellCurveVariance);
    }

    void Awake()
    {
        if (useOpenPSONET)
        {
            // Setup OpenPSO.NET
            if (algorithm == Algorithm.SSPSO)
            {
                throw new NotImplementedException();
            }

            OpenPSO.Lib.IFunction ifunction = null;
            switch (problem)
            {
                case Problem.Sphere:
                    ifunction = new OpenPSO.Functions.Sphere();
                    break;
                case Problem.Quadric:
                    ifunction = new OpenPSO.Functions.Quadric();
                    break;
                case Problem.Hyper:
                    ifunction = new OpenPSO.Functions.HyperEllipsoid();
                    break;
                case Problem.Rastrigin:
                    ifunction = new OpenPSO.Functions.Rastrigin();
                    break;
                case Problem.Griewank:
                    ifunction = new OpenPSO.Functions.Griewank();
                    break;
                case Problem.Schaffer:
                    ifunction = new OpenPSO.Functions.Schaffer2();
                    break;
                case Problem.Weierstrass:
                    ifunction = new OpenPSO.Functions.Weierstrass();
                    break;
                case Problem.Ackley:
                    ifunction = new OpenPSO.Functions.Ackley();
                    break;
                case Problem.ShiftedQuadricWithNoise:
                    throw new NotImplementedException();
                case Problem.RotatedGriewank:
                    throw new NotImplementedException();
                case Problem.PerlinLandscape:
                    ifunction = new OpenPSO.Functions.PerlinLandscape(perlinOctaves, perlinAmplitude, perlinAmplitudePerOctave,
                                                                      perlinFrequency.x, perlinFrequency.y, perlinFrequencyPerOctave,
                                                                      perlinOffset.x, perlinOffset.y);
                    break;
                case Problem.ImageValue:
                    if (!image.isReadable)
                    {
                        throw new ArgumentException("Image must be readable to use with Image Value function!");
                    }
                    ifunction = new PSOImageValueEvaluator(image, 
                                                           new Rect(initialX.x, initialX.x, initialX.y - initialX.x, initialX.y - initialX.x), 
                                                           (invertImage) ?(-1.0):(1.0));
                    break;
                case Problem.ImageSaturation:
                    if (!image.isReadable)
                    {
                        throw new ArgumentException("Image must be readable to use with Image Saturation function!");
                    }
                    ifunction = new PSOImageSaturationEvaluator(image,
                                                                new Rect(initialX.x, initialX.x, initialX.y - initialX.x, initialX.y - initialX.x),
                                                                (invertImage) ? (-1.0) : (1.0));
                    break;
                case Problem.RalphBellCurveMean:
                case Problem.RalphBellCurveVariance:
                    if (!image.isReadable)
                    {
                        throw new ArgumentException("Image must be readable to use with Image Saturation function!");
                    }
                    ifunction = new PSOImageRalphBellCurve(image,
                                                           new Rect(initialX.x, initialX.x, initialX.y - initialX.x, initialX.y - initialX.x),
                                                           (invertImage) ? (-1.0) : (1.0),
                                                           sampleRadius, problem == Problem.RalphBellCurveMean, useStimulus);
                    break;
                default:
                    throw new NotImplementedException();
            }

            OpenPSO.Lib.ITopology itopology = null;
            switch (topology)
            {
                case Topology.StaticRing1D:
                    throw new NotImplementedException();
                case Topology.StaticGrid2D:
                    switch (neighborhoodType)
                    {
                        case NeightborhoodType.Moore:
                            itopology = new OpenPSO.Lib.Topologies.MooreGridTopology(dimension.x, dimension.y);
                            break;
                        case NeightborhoodType.VN:
                            itopology = new OpenPSO.Lib.Topologies.VonNeumannGridTopology(dimension.x, dimension.y);
                            break;
                        case NeightborhoodType.Ring:
                            throw new NotImplementedException();
                        default:
                            break;
                    }
                    break;
                case Topology.StaticGraph:
                    throw new NotImplementedException();
                case Topology.Global:
                    itopology = new OpenPSO.Lib.Topologies.GlobalTopology(nParticles);
                    break;
                default:
                    break;
            }

            OpenPSO.Lib.PSO pso = new OpenPSO.Lib.PSO(
                p => true,
                p => omega,
                p => c1,
                p => c2,
                p => -xMax, 
                p => xMax,
                p => vMax,
                initialX.x,
                initialX.y,
                ifunction,
                2,
                maxEvaluations,
                stopCriterion,
                continueAfterStop,
                itopology,
                seed);

            // Setup PSORender
            var psoRender = GetComponent<PSORender>();
            psoRender.enabled = true;
            psoRender.runData = null;
            psoRender.runText = "";
            psoRender.topologyData = null;
            psoRender.functionData = null;
            psoRender.functionText = "";
            psoRender.function = ifunction;
            psoRender.functionSamples = functionSamplingSize;
            psoRender.functionSamplingInterval = new Vector2(initialX.x, initialX.y);
            psoRender.pso = pso;
            if (IsImageAndPSONET())
            {
                psoRender.texture = image;
            }
            else
            {
                psoRender.texture = null;
            }
        }
        else
        {
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(@"Assets/PSORun/current.ini"))
            {
                file.WriteLine("[pso]");
                file.WriteLine("");
                file.WriteLine("instrument = 1");
                file.WriteLine("function_points = " + functionSamplingSize);
                file.WriteLine("n_runs = 1");
                file.WriteLine("max_t = " + maxIterations);
                file.WriteLine("max_evaluations = " + maxEvaluations);
                file.WriteLine("algorithm = " + (int)algorithm);
                file.WriteLine("gbest = " + ((topology == Topology.Global) ? ("1") : ("0")));
                file.WriteLine("problem = " + (int)problem);
                file.WriteLine("Xmax = " + xMax);
                file.WriteLine("Vmax = " + vMax);
                file.WriteLine("chi = " + chi);
                file.WriteLine("omega = " + omega);
                file.WriteLine("c = " + c1);
                file.WriteLine("numberVariables = 2");
                file.WriteLine("iWeightStrategy = " + (int)weightStrategy);
                file.WriteLine("cStrategy = " + (int)cStrategy);
                file.WriteLine("assyInitialization = " + ((assymetricInitialization) ? ("1") : ("0")));
                file.WriteLine("initialXmin = " + initialX.x);
                file.WriteLine("initialXmax = " + initialX.y);
                file.WriteLine("numExtraRndNeighs = " + numExtraRandomNeighbours);
                file.WriteLine("crit = " + stopCriterion);
                file.WriteLine("crit_keep_going = " + ((continueAfterStop) ? ("1") : ("0")));
                file.WriteLine("bsf_save_period = 1000");
                switch (watershedStrategy)
                {
                    case WatershedStrategy.None: file.WriteLine("watershed_strategy = none"); break;
                    case WatershedStrategy.WorstSoFar: file.WriteLine("watershed_strategy = worst_so_far"); break;
                    case WatershedStrategy.WorstLastIteration: file.WriteLine("watershed_strategy = worst_last_iter"); break;
                    case WatershedStrategy.BestWorst: file.WriteLine("watershed_strategy = best_worst"); break;
                }
                file.WriteLine("");
                file.WriteLine("[topology]");
                file.WriteLine("");
                switch (topology)
                {
                    case Topology.StaticRing1D:
                        file.WriteLine("type = staticring1d");
                        file.WriteLine("nparticles = " + nParticles);
                        file.WriteLine("radius = " + radius);
                        break;
                    case Topology.StaticGrid2D:
                        file.WriteLine("type = staticgrid2d");
                        file.WriteLine("xdim = " + dimension.x);
                        file.WriteLine("ydim = " + dimension.y);
                        switch (neighborhoodType)
                        {
                            case NeightborhoodType.Moore: file.WriteLine("neighbordhood = MOORE"); break;
                            case NeightborhoodType.VN: file.WriteLine("neighbordhood = VN"); break;
                            case NeightborhoodType.Ring: file.WriteLine("neighbordhood = RING"); break;
                            default:
                                break;
                        }
                        break;
                    case Topology.StaticGraph:
                        file.WriteLine("type = staticgraph");
                        file.WriteLine("tgf_file = " + tgf_filename);
                        break;
                    default:
                        break;
                }
            }

            // Run open PSO
            psoRun = new System.Diagnostics.Process();
            psoRun.StartInfo.FileName = Application.dataPath + "/PSORun/runpso.exe";
            psoRun.StartInfo.Arguments = Application.dataPath + "/PSORun/current.ini" + " " + seed;
            psoRun.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            if (psoRun.Start())
            {
                Debug.Log("Running simulation...");

                var psoRender = GetComponent<PSORender>();
                psoRender.enabled = false;
                psoRender.runData = null;
                psoRender.functionData = null;
                psoRender.topologyData = null;
            }
            else
            {
                psoRun = null;
            }
        }
    }

    void Update()
    {
        if (psoRun != null)
        {
            if (psoRun.HasExited)
            {
                if (psoRun.ExitCode == 0)
                {
                    // Get the data, dump into the PSORender and start it
                    Debug.Log("Finished simulation!");

                    var psoRender = GetComponent<PSORender>();
                    psoRender.functionText = File.ReadAllText(Application.dataPath + "/PSORun/current.ini_function.txt");
                    psoRender.runText = File.ReadAllText(Application.dataPath + "/PSORun/current.ini_run0.txt");
                    psoRender.topologyText = File.ReadAllText(Application.dataPath + "/PSORun/current.ini_run0_topology.txt");
                    psoRender.enabled = true;
                }
                else
                {
                    Debug.LogError("Failed to run simulation!");
                }

                Destroy(this);
            }
        }
    }
}
