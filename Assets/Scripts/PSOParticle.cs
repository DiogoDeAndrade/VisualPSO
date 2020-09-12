using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSOParticle : MonoBehaviour
{
    struct UpdateItem
    {
        public float    time;
        public Vector3  position;
    };

    public int          particleId;
    public Color        color = Color.white;
    public float        scale = 1.0f;
    public Transform[]  scalableObjects;
    public float        totalTime;
    public PSORender    manager;
    public LineRenderer connectionPrefab;

    List<UpdateItem>    positions;
    int                 index;
    float               elapsedTime = 0.0f;
    TrailRenderer       trailRenderer;

    struct Connection
    {
        public PSOParticle  particle;
        public LineRenderer lineRenderer;
    }
    List<Connection>    connections;

    void Start()
    {
        index = 0;

        trailRenderer = GetComponent<TrailRenderer>();
        if (trailRenderer)
        {
            trailRenderer.time = 1.0f * scale; // totalTime * 0.0005f;
            trailRenderer.startColor = color;
            trailRenderer.endColor = new Color(color.r, color.g, color.a, 0.0f);
            trailRenderer.widthMultiplier = scale;
        }
        if (scalableObjects != null)
        {
            foreach (var t in scalableObjects)
            {
                t.localScale = t.localScale * scale;
            }
        }

        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        if (mr)
        {
            Color c = color;
            c = c * 5.0f;
            c.a = color.a;

            mr.material = new Material(mr.material);
            mr.material.SetColor("_Color", c);
        }

        UpdateConnections();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime * manager.playSpeed;

        if (trailRenderer)
        {
            trailRenderer.time = (1.0f * scale) / manager.playSpeed;
        }

        if ((index + 1) < positions.Count)
        {
            if (elapsedTime >= positions[index + 1].time)
            {
                index = index + 1;
            }

            if ((index + 1) < positions.Count)
            {
                Vector3 p1 = positions[index].position;
                Vector3 p2 = positions[index + 1].position;

                float t = (elapsedTime - positions[index].time) / (positions[index + 1].time - positions[index].time);
                transform.position = Vector3.Lerp(p1, p2, t);
            }
        }
    }

    public void AddUpdateAction(float time, float x, float y, float z)
    {
        if (positions == null) positions = new List<UpdateItem>();

        positions.Add(new UpdateItem { time = time, position = new Vector3(x, y, z) });
    }

    public void AddConnection(PSOParticle particle)
    {
        if (connectionPrefab == null) return;

        // Just keep one direction of the pair of particles (halves the line renderers needed)
        if (particle.particleId < particleId) return;

        if (connections == null) connections = new List<Connection>();

        Connection conn = new Connection
        {
            particle = particle,
            lineRenderer = Instantiate(connectionPrefab, transform)
        };

        connections.Add(conn);
    }

    private void LateUpdate()
    {
        UpdateConnections();
    }

    void UpdateConnections()
    {
        if (connections == null) return;

        foreach (var c in connections)
        {
            c.lineRenderer.SetPositions(new Vector3[2] { transform.position, c.particle.transform.position });
        }
    }
}
