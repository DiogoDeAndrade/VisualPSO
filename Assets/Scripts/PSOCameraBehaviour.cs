using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSOCameraBehaviour : MonoBehaviour
{
    [HideInInspector] public bool   autoStart = true;
    [HideInInspector] public float  scale = 1.0f;
    [HideInInspector] public bool   testOcclusion = false;

    public virtual bool Restart(float estimatedTime)
    {
        return false;
    }
}
