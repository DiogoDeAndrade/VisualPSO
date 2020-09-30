using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSORender : MonoBehaviour
{
    public TextAsset    runData;
    public TextAsset    functionData;
    public TextAsset    topologyData;
    public float        timePerIteration = 1.0f;
    public PSOParticle  particlePrefab;
    public PSOFunction  functionPrefab;
    public Material     materialOverride;
    public Gradient     colorParticles;
    public bool         moveY = false;
    public bool         fogOfFunction = false;
    public float        yScale = 1.0f;
    public float        particleScale = 1.0f;
    public bool         displayConnectivity = true;
    [Range(0.0f, 10.0f)]
    public float        playSpeed = 1.0f;

    [Header("References")]
    public Camera       mainCamera;

    [Header("Runtime")]
    public Rect         boundary;
    public Vector2      extentsY;
    public float        totalTime;

    List<PSOParticle> particles;
    float             elapsedTime;

    [HideInInspector] public string functionText = "";
    [HideInInspector] public string runText = "";
    [HideInInspector] public string topologyText = "";

    [HideInInspector] public OpenPSO.Lib.IFunction  function;
    [HideInInspector] public int                    functionSamples;
    [HideInInspector] public Vector2                functionSamplingInterval;

    [HideInInspector] public OpenPSO.Lib.PSO        pso;

    [HideInInspector] public Texture2D              texture;

    void Start()
    {
        particles = new List<PSOParticle>();

        var splitFile = new string[] { "\r\n", "\r", "\n" };
        var splitLine = new string[] { ";" };

        string rd = (runData == null)?(runText):(runData.text);

        var lines = rd.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);

        float x1 = float.MaxValue;
        float z1 = float.MaxValue;
        float x2 = -float.MaxValue;
        float z2 = -float.MaxValue;

        extentsY.Set(float.MaxValue, -float.MaxValue);

        totalTime = 0.0f;
        boundary.Set(0, 0, 0, 0);

        if (lines.Length > 1)
        {
            for (int idx = 1; idx < lines.Length; idx++)
            {
                var line = lines[idx];
                var tokens = line.Split(splitLine, System.StringSplitOptions.None);
                int action = int.Parse(tokens[0]);
                int iteration = int.Parse(tokens[1]);
                int particleId = int.Parse(tokens[2]);
                float x = float.Parse(tokens[3]);
                float z = float.Parse(tokens[4]);
                float y = float.Parse(tokens[5]);

                x1 = Mathf.Min(x1, x);
                x2 = Mathf.Max(x2, x);
                z1 = Mathf.Min(z1, z);
                z2 = Mathf.Max(z2, z);

                extentsY.x = Mathf.Min(extentsY.x, y);
                extentsY.y = Mathf.Max(extentsY.y, y);

                totalTime = Mathf.Max(totalTime, iteration * timePerIteration);

                if (particles.Count <= particleId)
                {
                    for (int i = particles.Count; i <= particleId; i++) particles.Add(null);
                }

                if (particles[particleId] == null)
                {
                    // Create new particle
                    particles[particleId] = Instantiate(particlePrefab);
                    particles[particleId].name = "Particle " + particleId;
                    particles[particleId].particleId = particleId;
                    particles[particleId].transform.position = new Vector3(x, 0, z);
                    particles[particleId].manager = this;
                }

                if (action == 0)
                {
                    if (!moveY) y = 0.0f;
                    particles[particleId].AddUpdateAction(iteration * timePerIteration, x, y * yScale, z);
                }
                else
                {
                    Debug.Assert(false, "Unknown action!");
                }
            }

            boundary.Set(x1, z1, x2 - x1, z2 - z1);
        }
        else if (pso != null)
        {
            // Runtime PSO

            // Create particles
            var particleData = pso.Topology.Particles;

            int particleId = 1;
            foreach (var p in particleData)
            {
                float x = (float)p.Position[0];
                float y = (float)p.Position[1];

                var particle = Instantiate(particlePrefab);
                particle.name = "Particle " + particleId;
                particle.particleId = particleId;
                particle.transform.position = new Vector3(x, (float)p.Fitness * yScale, y);
                particle.manager = this;

                particleId++;
                particles.Add(particle);
            }

            boundary.Set((float)pso.InitXMin, (float)pso.InitXMin, (float)(pso.InitXMax - pso.InitXMin), (float)(pso.InitXMax - pso.InitXMin));
        }

        float maxExtent = Mathf.Max(boundary.height, boundary.width);
        if (mainCamera.orthographic)
        {
            mainCamera.orthographicSize = (maxExtent * 0.5f) * 1.05f;
        }

        float scale = maxExtent / 150.0f;
        foreach (var particle in particles)
        {
            particle.scale = scale * particleScale;
            particle.color = colorParticles.Evaluate(Random.Range(0.0f, 1.0f));
            particle.totalTime = totalTime;
            particle.offsetY *= scale * particleScale;
        }

        if (displayConnectivity)
        {
            if ((topologyData) || (topologyText != ""))
            {
                string td = (topologyData == null) ? (topologyText) : (topologyData.text);
                lines = td.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);

                for (int idx = 0; idx < lines.Length; idx++)
                {
                    var line = lines[idx];
                    var tokens = line.Split(splitLine, System.StringSplitOptions.None);

                    var particleId = int.Parse(tokens[0]);

                    for (int i = 1; i < tokens.Length; i++)
                    {
                        particles[particleId].AddConnection(particles[int.Parse(tokens[i])]);
                    }
                }
            }
            else if (pso != null)
            {
                var topology = pso.Topology;
                var particleData = topology.Particles;

                int particleIndex = 0;
                foreach (var p in particleData)
                {
                    foreach (var pNeigh in topology.GetNeighbors(p))
                    {
                        int n = pNeigh.id;
                        particles[particleIndex].AddConnection(particles[n]);
                    }

                    particleIndex++;
                }
            }
        }      

        if (functionPrefab)
        {
            var visFunction = Instantiate(functionPrefab);
            visFunction.manager = this;
            visFunction.SetFoF(fogOfFunction);

            if ((functionData) || (functionText != ""))
            {
                string fd = (functionData == null) ? (functionText) : (functionData.text);

                visFunction.Parse(fd, yScale);
            }
            else if (function != null)
            {
                visFunction.Parse(function, functionSamples, functionSamplingInterval, yScale);
                extentsY.x = visFunction.minY;
                extentsY.y = visFunction.maxY;
            }

            visFunction.SetMaterial(materialOverride, texture);
        }
    }

    public List<PSOParticle> GetParticles()
    {
        return particles;
    }

    public PSOParticle GetRandomParticle()
    {
        if (particles == null) return null;

        return particles[Random.Range(0, particles.Count)];
    }

    public PSOParticle GetBestParticle()
    {
        if (particles == null) return null;

        float       val = float.MaxValue;
        PSOParticle particle = null;

        foreach (var p in particles)
        {
            if (p.transform.position.y < val)
            {
                particle = p;
                val = p.transform.position.y;
            }
        }

        return particle;
    }

    public PSOParticle GetWorstParticle()
    {
        if (particles == null) return null;

        float val = -float.MaxValue;
        PSOParticle particle = null;

        foreach (var p in particles)
        {
            if (p.transform.position.y > val)
            {
                particle = p;
                val = p.transform.position.y;
            }
        }

        return particle;
    }

    private void Update()
    {
        if (pso != null)
        {
            elapsedTime += Time.deltaTime * playSpeed;

            float timeToSimulate = 0.0f;
            while (elapsedTime >= timePerIteration)
            {
                pso.UpdatePopData();
                pso.UpdateParticles();
                timeToSimulate += timePerIteration;
                elapsedTime -= timePerIteration;
            }

            if (timeToSimulate > 0)
            {
                var particleData = pso.Topology.Particles;

                int particleIndex = 0;
                foreach (var p in particleData)
                {
                    float x = (float)p.Position[0];
                    float y = (float)p.Fitness * yScale;
                    float z = (float)p.Position[1];

                    particles[particleIndex].SetTarget(x, y, z, timeToSimulate);

                    particleIndex++;
                }
            }
        }
    }
}
