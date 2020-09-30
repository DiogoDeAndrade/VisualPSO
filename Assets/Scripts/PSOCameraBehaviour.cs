using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSOCameraBehaviour : MonoBehaviour
{
    [HideInInspector] public bool   autoStart = true;
    [HideInInspector] public float  scale = 1.0f;

    public virtual void Restart(float estimatedTime)
    {

    }
}
