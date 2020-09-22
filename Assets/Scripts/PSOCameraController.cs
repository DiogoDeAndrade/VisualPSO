using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PSOCameraController : MonoBehaviour
{
    public PSOCameraBehaviour   startBehaviour;
    public bool                 autoSwitch;
    [ShowIf("autoSwitch")]
    public float                minTime;
    [ShowIf("autoSwitch")]
    public float                maxTime;

    PSOCameraBehaviour[]    cameraBehaviours;

    PSOCameraBehaviour  current;    
    float               switchTimer;

    void Awake()
    {
        Camera camera = GetComponent<Camera>();
        camera.orthographic = false;

        cameraBehaviours = GetComponentsInChildren<PSOCameraBehaviour>(true);

        foreach (var cb in cameraBehaviours)
        {
            cb.enabled = false;
            cb.autoStart = false;
        }
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        if (startBehaviour != null)
        {
            Run(startBehaviour);
        }
    }

    void Update()
    {
        if (autoSwitch)
        {
            switchTimer -= Time.deltaTime;
            if (switchTimer < 0)
            {
                Stop();
            }
        }

        if (current == null)
        {
            // Choose a random one
            Run(cameraBehaviours[Random.Range(0, cameraBehaviours.Length)]);
        }
    }

    void Run(PSOCameraBehaviour behaviour)
    {
        if (autoSwitch)
        {
            switchTimer = Random.Range(minTime, maxTime);
        }
        else
        {
            switchTimer = 1.0f;
        }

        current = behaviour;
        current.enabled = true;
        current.Restart(switchTimer * 0.75f);
    }

    void Stop()
    {
        if (current)
        {
            current.enabled = false;
            current = null;
        }
    }
}
