using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PSOCB_Random : PSOCameraBehaviour
{
    public enum TargetParticle { None, Best, Worst, Random };

    public float innerRadius = 10.0f;
    public float outerRadius = 40.0f;
    public bool  zoomIn = false;
    [ShowIf("zoomIn")]
    public float zoomInSpeed = 1.0f;
    public bool  rotate = false;
    [ShowIf("rotate")]
    public float rotationSpeed = 5.0f;

    public TargetParticle   targetParticle;
    [ShowIf("HasTargetParticle")]
    public bool             usePrediction;
    [ShowIf("HasTargetParticle")]
    public float            maxSpeed = 5.0f;

    Vector3         startLookPos, targetPos;
    float           elapsedTime = 0.0f;
    Transform       particleTransform;
    System.Random   rndGen;

    bool HasTargetParticle()
    {
        return targetParticle != TargetParticle.None;
    }

    void Start()
    {
        if (autoStart)
        {
            Invoke("Restart", 0.25f);
        }
    }

    override public bool Restart(int seed, float estimatedTime)
    {
        int tries = 0;
        int maxTries = 25;
        rndGen = new System.Random(seed);

        while (tries < maxTries)
        {
            elapsedTime = 0;

            PSORender psoRender = FindObjectOfType<PSORender>();

            startLookPos = new Vector3(rndGen.Range(-1, 1), rndGen.Range(0.5f, 1.5f), rndGen.Range(-1, 1)).normalized * outerRadius * scale;

            startLookPos.y = Mathf.Max(startLookPos.y, psoRender.extentsY.y);

            PSOParticle particle = null;

            switch (targetParticle)
            {
                case TargetParticle.None:
                    targetPos = rndGen.onUnitSphere() * innerRadius * scale;
                    if (targetPos.y < 0) targetPos.y = Mathf.Abs(targetPos.y);
                    break;
                case TargetParticle.Best:
                    particle = psoRender.GetBestParticle();
                    break;
                case TargetParticle.Worst:
                    particle = psoRender.GetBestParticle();
                    break;
                case TargetParticle.Random:
                    int index = rndGen.Range(0, psoRender.GetParticleCount());
                    particle = psoRender.GetParticle(index);
                    break;
                default:
                    break;
            }

            transform.position = startLookPos;

            if (particle)
            {
                if (usePrediction)
                {
                    targetPos = particle.Predict(estimatedTime);
                }
                else
                {
                    particleTransform = particle.transform;
                    targetPos = particleTransform.position;
                }

                Vector3 toTarget = targetPos - startLookPos;
                Vector3 dir = toTarget.normalized;
                float maxDist = toTarget.magnitude;

                if (dir.y < -0.1f)
                {
                    // Check raycast
                    if (!Physics.Raycast(startLookPos, dir, maxDist))
                    {
                        break;
                    }
                }

                tries++;
            }
            else
            {
                Vector3 toTarget = targetPos - startLookPos;
                Vector3 dir = toTarget.normalized;
                float maxDist = toTarget.magnitude;

                if (dir.y < -0.1f)
                {
                    break;
                }
            }
        }

        if (tries == maxTries)
        {
            return false;
        }

        FixedUpdate();

        return true;
    }

    void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime;

        if (rotate)
        {
            startLookPos = Quaternion.Euler(0, rotationSpeed * Time.fixedDeltaTime * Mathf.Deg2Rad, 0) * startLookPos;
        }

        if (zoomIn)
        {
            transform.position = startLookPos + transform.forward * zoomInSpeed * scale * elapsedTime;
        }

        if (particleTransform)
        {
            targetPos = Vector3.MoveTowards(targetPos, particleTransform.position, maxSpeed * Time.fixedDeltaTime);
        }

        transform.rotation = Quaternion.LookRotation((targetPos - transform.position).normalized, Vector3.up);
    }
}
