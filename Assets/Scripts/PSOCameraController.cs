﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PSOCameraController : MonoBehaviour
{
    public PSOCameraBehaviour   startBehaviour;
    public float                globalScale = 1.0f;
    public bool                 testOcclusion = false;
    public bool                 autoSwitch;
    [ShowIf("autoSwitch")]
    public float                minTime;
    [ShowIf("autoSwitch")]
    public float                maxTime;

    PSOCameraBehaviour[]    cameraBehaviours;

    PSOCameraBehaviour  current;    
    float               switchTimer;
    System.Random       rndGen;

    void Awake()
    {
        Camera camera = GetComponent<Camera>();
        camera.orthographic = false;

        cameraBehaviours = GetComponentsInChildren<PSOCameraBehaviour>(true);

        foreach (var cb in cameraBehaviours)
        {
            cb.enabled = false;
            cb.autoStart = false;
            cb.scale = globalScale;
            cb.testOcclusion = testOcclusion;
        }

        rndGen = new System.Random(0);
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);

        switchTimer = rndGen.Range(minTime, maxTime);
        if (startBehaviour)
        {
            current = startBehaviour;
            current.enabled = true;
            current.Restart(rndGen.Next(), switchTimer * 0.75f);
        }
    }

    public void SetSeed(int seed)
    {
        rndGen = new System.Random(seed);
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
            SelectNew();
        }
    }    

    void SelectNew()
    {
        if (autoSwitch)
        {
            switchTimer = rndGen.Range(minTime, maxTime);
        }
        else
        {
            switchTimer = 1.0f;
        }

        int nTries = 0;
        int maxTries = 5;

        while (nTries < maxTries)
        {
            PSOCameraBehaviour behaviour = cameraBehaviours[rndGen.Range(0, cameraBehaviours.Length)];

            current = behaviour;
            current.enabled = true;
            if (current.Restart(rndGen.Next(), switchTimer * 0.75f)) break;

            nTries++;
        }
    }

    void Stop()
    {
        if (current)
        {
            current.enabled = false;
            current = null;
        }
    }

    public void SetScale(float s)
    {
        globalScale = s;
        foreach (var cameraBehaviour in cameraBehaviours)
        {
            cameraBehaviour.scale = s;
        }
    }
}
