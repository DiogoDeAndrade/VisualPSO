using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSORender : MonoBehaviour
{
    public PSOParticle  particlePrefab;
    public PSOFunction  functionPrefab;
    public Material     materialOverride;
    public Gradient     colorParticles;
    public float        timePerIteration = 0.1f;
    public bool         fogOfFunction = false;
    public float        yScale = 1.0f;
    public float        particleScale = 1.0f;
    public float        trailScale = 1.0f;
    public bool         displayConnectivity = true;
    [Range(0.0f, 10.0f)]
    public float        playSpeed = 1.0f;

    [Header("References")]
    public Camera       mainCamera;

    [HideInInspector] public Rect         boundary;
    [HideInInspector] public Vector2      extentsY;
    [HideInInspector] public float        totalTime;

    List<PSOParticle> particles;
    float             elapsedTime;

    [HideInInspector] public OpenPSO.Lib.IFunction  function;
    [HideInInspector] public int                    functionSamples;
    [HideInInspector] public Vector2                functionSamplingInterval;

    [HideInInspector] public OpenPSO.Lib.PSO        pso;

    [HideInInspector] public Texture2D              texture;

    void Start()
    {
        particles = new List<PSOParticle>();

        extentsY.Set(float.MaxValue, -float.MaxValue);

        totalTime = 0.0f;
        boundary.Set(0, 0, 0, 0);

        if (pso != null)
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
            particle.trailScale = trailScale;
        }

        if (displayConnectivity)
        {
            if (pso != null)
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

            if (function != null)
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

    public int GetParticleCount()
    {
        if (particles == null) return 0;

        return particles.Count;
    }

    public PSOParticle GetParticle(int index)
    {
        if (particles == null) return null;

        return particles[index];
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
