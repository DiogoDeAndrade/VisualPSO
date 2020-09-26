using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandLineProcessor : MonoBehaviour
{
    public GameObject[] presets;
    public PSOConfig    randomPreset;
    public PSOConfig    customPreset;
    public Texture2D[]  images;
    public Material[]   materialsImage;
    public Material[]   materialsLandscape;

    void Start()
    {
        float       speed = 1.0f;
        bool        connectivity = false;
        GameObject  activeObject = null;
        float       scale = 1.0f;
        bool        isCustom = false;
        int         materialPreset = 0;

        customPreset.perlinOffset.x = UnityEngine.Random.Range(-1000.0f, 1000.0f);
        customPreset.perlinOffset.y = UnityEngine.Random.Range(-1000.0f, 1000.0f);

        string[] args = System.Environment.GetCommandLineArgs();
//        string[] args = "-imagesaturation -image0 -material1".Split();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("-preset"))
            {
                if (int.TryParse(args[i].Substring(7), out int presetIndex))
                {
                    activeObject = presets[presetIndex];
                }
            }
            else if (args[i].StartsWith("-speed"))
            {
                if (float.TryParse(args[i].Substring(6), out speed))
                {
                }
            }
            else if (args[i] == "-random")
            {
                activeObject = randomPreset.gameObject;
                MakeRandom();
            }
            else if (args[i] == "-connectivity")
            {
                connectivity = true;
            }
            else if (args[i] == "-landscape") { customPreset.problem = PSOConfig.Problem.PerlinLandscape; isCustom = true; }
            else if (args[i] == "-imagevalue") { customPreset.problem = PSOConfig.Problem.ImageValue; isCustom = true; }
            else if (args[i] == "-imagesaturation") { customPreset.problem = PSOConfig.Problem.ImageSaturation; isCustom = true; }
            else if (args[i].StartsWith("-image"))
            {
                if (int.TryParse(args[i].Substring(6), out int imageSelection))
                {
                    customPreset.image = images[imageSelection];
                    customPreset.negateImage = true;
                }
            }
            else if (args[i].StartsWith("-material"))
            {
                int.TryParse(args[i].Substring(9), out materialPreset);
            }
            else if (args[i].StartsWith("-octaves"))
            {
                int.TryParse(args[i].Substring(8), out customPreset.perlinOctaves);
            }
            else if (args[i].StartsWith("-amplitude"))
            {
                float.TryParse(args[i].Substring(10), out customPreset.perlinAmplitude);
            }
            else if (args[i].StartsWith("-frequency"))
            {
                float.TryParse(args[i].Substring(10), out customPreset.perlinFrequency.x);
                customPreset.perlinFrequency.y = customPreset.perlinFrequency.x;
            }
            else if (args[i].StartsWith("-scale"))
            {
                float.TryParse(args[i].Substring(6), out scale);
            }
            else
            {
                Console.WriteLine($"Unknown argument ${args[i]}");
            }
        }

        if (isCustom)
        {
            activeObject = customPreset.gameObject;
        }
        else if (activeObject == null)
        {
            activeObject = randomPreset.gameObject;
            MakeRandom();
        }

        activeObject.SetActive(true);

        PSORender psoRender = activeObject.GetComponent<PSORender>();
        psoRender.playSpeed = speed;
        psoRender.displayConnectivity = connectivity;

        if (isCustom)
        {
            if ((customPreset.problem == PSOConfig.Problem.ImageSaturation) || (customPreset.problem == PSOConfig.Problem.ImageValue))
            {
                psoRender.materialOverride = materialsImage[materialPreset];
                psoRender.yScale = -20.0f * scale;                
            }
            else
            {
                psoRender.materialOverride = materialsLandscape[materialPreset];
                psoRender.yScale = scale;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(0);
        }
    }

    void MakeRandom()
    {
        var psoRender = randomPreset.GetComponent<PSORender>();

        var rnd = new System.Random((int)DateTime.UtcNow.Ticks);

        randomPreset.seed = (int)DateTime.UtcNow.Ticks;
        randomPreset.gBest = rnd.Range(0, 100) < 50;

        int p = rnd.Range(0, 100);
        if (p < 50)
        {
            randomPreset.problem = PSOConfig.Problem.PerlinLandscape;

            randomPreset.perlinOctaves = rnd.Range(4, 10);
            randomPreset.perlinAmplitude = rnd.Range(15.0f, 25.0f);
            randomPreset.perlinFrequency.x = rnd.Range(0.02f, 0.1f);
            randomPreset.perlinFrequency.y = randomPreset.perlinFrequency.x * rnd.Gaussian(1.0f, 0.1f);
            randomPreset.perlinAmplitudePerOctave = rnd.Gaussian(0.5f, 0.1f);
            randomPreset.perlinFrequencyPerOctave = rnd.Gaussian(2.0f, 0.5f);
            randomPreset.perlinOffset.x = rnd.Range(-1000.0f, 1000.0f);
            randomPreset.perlinOffset.y = rnd.Range(-1000.0f, 1000.0f);

            psoRender.materialOverride = materialsLandscape[rnd.Range(0, materialsLandscape.Length)];
            psoRender.yScale = 1.0f;
        }
        else 
        {
            if (p < 75) randomPreset.problem = PSOConfig.Problem.ImageSaturation;
            else randomPreset.problem = PSOConfig.Problem.ImageValue;

            randomPreset.image = images[rnd.Range(0, images.Length)];
            randomPreset.negateImage = true;

            psoRender.yScale = -rnd.Gaussian(20.0f, 4.0f);
            psoRender.materialOverride = materialsImage[rnd.Range(0, materialsImage.Length)];
        }
    }
}
