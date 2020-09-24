using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

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
    public float        offsetY = 0;
    public bool         colorByGradient = false;
    [ShowIf("colorByGradient"), ColorUsage(true, true)]
    public Color        improvingColor = Color.green;
    [ShowIf("colorByGradient"), ColorUsage(true, true)]
    public Color        worseningColor = Color.red;

    List<UpdateItem>    positions;
    int                 index;
    float               elapsedTime = 0.0f;
    TrailRenderer       trailRenderer;
    Material            material;
    Color               currentColor;
    Vector3             startPos;
    float               timeForThisStep;
    float               timerForThisStep;
    Vector3             targetPosition;
    Vector3             currentVelocity;

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

            material = new Material(mr.material);
            material.SetColor("_Color", c);

            mr.material = material;
        }

        if (colorByGradient)
        {
            SetColor(color);
        }

        UpdateConnections();

        startPos = transform.position;
        targetPosition = startPos;
        currentVelocity = Vector3.zero;
        timeForThisStep = manager.timePerIteration;
        timerForThisStep = 0;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime * manager.playSpeed;
        timerForThisStep += Time.deltaTime * manager.playSpeed;

        if (trailRenderer)
        {
            trailRenderer.time = (1.0f * scale) / manager.playSpeed;
        }

        Vector3 oldPos = transform.position;

        if (positions == null)
        {
            // Runtime PSO
            Vector3 p1 = startPos;
            Vector3 p2 = targetPosition;

            if (Vector3.Distance(p1, p2) < 0.001f)
            {
                if (colorByGradient)
                {
                    var targetColor = improvingColor;
                    Color c = Color.Lerp(currentColor, targetColor, 0.15f);
                    SetColor(c);
                }
            }
            else
            {
                float t = timerForThisStep / timeForThisStep;
                transform.position = Vector3.Lerp(p1, p2, t) + Vector3.up * offsetY;

                if (colorByGradient)
                {
                    Vector3 delta = transform.position - oldPos;

                    Color targetColor;
                    if (delta.y <= 0.1)
                    {
                        targetColor = improvingColor;
                    }
                    else
                    {
                        targetColor = worseningColor;
                    }

                    Color c = Color.Lerp(currentColor, targetColor, 0.15f);
                    SetColor(c);
                }
            }
        }
        else
        {
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
                    transform.position = Vector3.Lerp(p1, p2, t) + Vector3.up * offsetY;

                    if (colorByGradient)
                    {
                        Vector3 delta = transform.position - oldPos;

                        Color targetColor;
                        if (delta.y <= 0.1)
                        {
                            targetColor = improvingColor;
                        }
                        else
                        {
                            targetColor = worseningColor;
                        }

                        Color c = Color.Lerp(currentColor, targetColor, 0.15f);
                        SetColor(c);
                    }
                }
                else
                {
                    if (colorByGradient)
                    {
                        var targetColor = improvingColor;
                        Color c = Color.Lerp(currentColor, targetColor, 0.15f);
                        SetColor(c);
                    }
                }
            }
            else
            {
                if (colorByGradient)
                {
                    var targetColor = improvingColor;
                    Color c = Color.Lerp(currentColor, targetColor, 0.15f);
                    SetColor(c);
                }
            }
        }

        currentVelocity = (transform.position - oldPos) / Time.deltaTime;
    }

    public void AddUpdateAction(float time, float x, float y, float z)
    {
        if (positions == null)
        {
            positions = new List<UpdateItem>();
            transform.position = new Vector3(x, y, z) + Vector3.up * offsetY;
    }

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

    void SetColor(Color c)
    {
        material.SetColor("_Color", c);
        trailRenderer.startColor = c;
        trailRenderer.endColor = new Color(c.r, c.g, c.a, 0.0f);

        currentColor = c;
    }

    public Vector3 Predict(float time)
    {
        if (positions == null)
        {
            return transform.position + currentVelocity * time;
        }
        else
        {
            float t = elapsedTime + time;

            for (int i = 1; i < positions.Count; i++)
            {
                if (positions[i].time >= t)
                {
                    Vector3 p1 = positions[i - 1].position;
                    Vector3 p2 = positions[i].position;
                    float tt = (t - positions[i - 1].time) / (positions[i].time - positions[i - 1].time);
                    return Vector3.Lerp(p1, p2, tt);
                }
            }
        }

        return Vector3.zero;
    }

    public void SetTarget(float x, float y, float z, float time)
    {
        startPos = transform.position - Vector3.up * offsetY;
        targetPosition = new Vector3(x, y, z);
        timeForThisStep = time;
        timerForThisStep = 0.0f;
    }
}
