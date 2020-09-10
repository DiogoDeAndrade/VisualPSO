using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSORender : MonoBehaviour
{
    public TextAsset    runData;
    public TextAsset    functionData;
    public float        timePerIteration = 1.0f;
    public PSOParticle  particlePrefab;
    public PSOFunction  functionPrefab;
    public Gradient     colorParticles;
    public bool         moveY = false;
    public float        yScale = 1.0f;

    [Header("References")]
    public Camera       mainCamera;

    [Header("Runtime")]
    public Rect         boundary;
    public float        totalTime;

    List<PSOParticle> particles;
    
    void Start()
    {
        particles = new List<PSOParticle>();

        var splitFile = new string[] { "\r\n", "\r", "\n" };
        var splitLine = new string[] { ";" };
        var lines = runData.text.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);

        float x1 = float.MaxValue;
        float z1 = float.MaxValue;
        float x2 = -float.MaxValue;
        float z2 = -float.MaxValue;

        totalTime = 0.0f;

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

        float maxExtent = Mathf.Max(boundary.height, boundary.width);
        mainCamera.orthographicSize = (maxExtent * 0.5f) * 1.05f;

        float scale = maxExtent / 10.0f;
        foreach (var particle in particles)
        {
            particle.scale = scale;
            particle.color = colorParticles.Evaluate(Random.Range(0.0f, 1.0f));
            particle.totalTime = totalTime;
        }

        if ((functionData) && (functionPrefab))
        {
            var visFunction = Instantiate(functionPrefab);
            visFunction.Parse(functionData, yScale);
        }
    }

    void Update()
    {
        
    }
}
