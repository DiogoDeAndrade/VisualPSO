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

    public int      particleId;
    public Color    color = Color.white;
    public float    scale = 1.0f;
    public float    totalTime;

    List<UpdateItem> positions;
    int              index;
    float            elapsedTime = 0.0f;
    
    void Start()
    {
        index = 0;

        TrailRenderer tr = GetComponent<TrailRenderer>();
        if (tr)
        {
            tr.time = totalTime * 0.0005f;
            tr.startColor = color;
            tr.endColor = new Color(color.r, color.g, color.a, 0.0f);
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, scale * 0.125f);
    }
}
