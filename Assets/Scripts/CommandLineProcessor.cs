using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandLineProcessor : MonoBehaviour
{
    public GameObject[] presets;
    public GameObject[] experiments;
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
        bool        fof = false;
        float       limitX = 100.0f;
        int         seed = 12345;

        customPreset.perlinOffset.x = UnityEngine.Random.Range(-1000.0f, 1000.0f);
        customPreset.perlinOffset.y = UnityEngine.Random.Range(-1000.0f, 1000.0f);

        //string cmdLine = "-imagevalue -image\"d:\\temp\\test folder\\download.jpg\" -material3";
        //string cmdLine = "-weierstrass -material2";
        //string cmdLine = "-rastrigin -material2";
        //string cmdLine = "-experiment7";
        //string cmdLine = "-ackley -material3";
        string cmdLine = string.Join(" ", Environment.GetCommandLineArgs());

        var args = ParseText(cmdLine, ' ', '"');

        for (int i = 0; i < args.Count; i++)
        {
            if (args[i].StartsWith("-preset"))
            {
                if (int.TryParse(args[i].Substring(7), out int presetIndex))
                {
                    activeObject = presets[presetIndex];
                }
            }
            else if (args[i].StartsWith("-experiment"))
            {
                if (int.TryParse(args[i].Substring(11), out int experimentIndex))
                {
                    activeObject = experiments[experimentIndex - 1];
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
            else if (args[i] == "-sphere") { customPreset.problem = PSOConfig.Problem.Sphere; scale = 0.01f; isCustom = true; }
            else if (args[i] == "-quadric") { customPreset.problem = PSOConfig.Problem.Quadric; scale = 0.01f;  isCustom = true; }
            else if (args[i] == "-hyperellipsoid") { customPreset.problem = PSOConfig.Problem.Hyper; scale = 0.01f;  isCustom = true; }
            else if (args[i] == "-rastrigin") { customPreset.problem = PSOConfig.Problem.Rastrigin; scale = 0.1f; limitX = 10.0f; customPreset.vMax = 0.25f; isCustom = true; }
            else if (args[i] == "-griewank") { customPreset.problem = PSOConfig.Problem.Griewank; isCustom = true; }
            else if (args[i] == "-schaffer") { customPreset.problem = PSOConfig.Problem.Schaffer; limitX = 10.0f; customPreset.vMax = 0.25f; isCustom = true; }
            else if (args[i] == "-ackley") { customPreset.problem = PSOConfig.Problem.Ackley; scale = 0.25f;  limitX = 10.0f; customPreset.vMax = 0.25f; isCustom = true; }
            else if (args[i] == "-weierstrass") { customPreset.problem = PSOConfig.Problem.Weierstrass; scale = 0.25f;  limitX = 10.0f; customPreset.vMax = 0.25f; isCustom = true; }
            else if (args[i] == "-ralphmean") { customPreset.problem = PSOConfig.Problem.RalphBellCurveMean; scale = 20.0f; isCustom = true; }
            else if (args[i] == "-ralphvar") { customPreset.problem = PSOConfig.Problem.RalphBellCurveVariance; scale = 20.0f; isCustom = true; }
            else if (args[i] == "-fof") { fof = true; isCustom = true; }
            else if (args[i] == "-usestimulus") { customPreset.useStimulus = true; isCustom = true; }
            else if (args[i] == "-useresponse") { customPreset.useStimulus = false; isCustom = true; }
            else if (args[i].StartsWith("-sampleradius"))
            {
                int.TryParse(args[i].Substring(13), out customPreset.sampleRadius);
                isCustom = true;
            }
            else if (args[i].StartsWith("-image"))
            {
                if (int.TryParse(args[i].Substring(6), out int imageSelection))
                {
                    customPreset.image = images[imageSelection];
                    customPreset.invertImage = true;
                }
                else
                {
                    // Check if this is a path
                    var filename = args[i].Substring(6);
                    if (filename[0] == '"') filename = filename.Substring(1);
                    if (filename[filename.Length - 1] == '"') filename = filename.Substring(0, filename.Length - 1);

                    if (System.IO.File.Exists(filename))
                    {
                        // Load this file
                        var data = System.IO.File.ReadAllBytes(filename);
                        if (data.Length > 0)
                        {
                            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                            if (tex.LoadImage(data))
                            {
                                customPreset.image = tex;
                                customPreset.invertImage = true;
                            }
                        }
                    }
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
            else if (args[i].StartsWith("-chi"))
            {
                float.TryParse(args[i].Substring(4), out customPreset.chi);
            }
            else if (args[i].StartsWith("-w"))
            {
                float.TryParse(args[i].Substring(2), out customPreset.omega);
            }
            else if (args[i].StartsWith("-c"))
            {
                float.TryParse(args[i].Substring(2), out customPreset.c1);
                float.TryParse(args[i].Substring(2), out customPreset.c2);
            }
            else if (args[i].StartsWith("-c1"))
            {
                float.TryParse(args[i].Substring(2), out customPreset.c1);
            }
            else if (args[i].StartsWith("-c2"))
            {
                float.TryParse(args[i].Substring(2), out customPreset.c2);
            }
            else if (args[i].StartsWith("-vmax"))
            {
                float.TryParse(args[i].Substring(5), out customPreset.vMax);
            }
            else if (args[i].StartsWith("-rngseed"))
            {
                int.TryParse(args[i].Substring(8), out seed);
            }
            else
            {
                Console.WriteLine($"Unknown argument ${args[i]}");
            }
        }

        if (isCustom)
        {
            customPreset.initialX = new Vector2(-limitX, limitX);
            customPreset.xMax = limitX;

            var cameraController = FindObjectOfType<PSOCameraController>();
            cameraController.SetScale(limitX / 100.0f);

            activeObject = customPreset.gameObject;
        }
        else if (activeObject == null)
        {
            activeObject = randomPreset.gameObject;
            MakeRandom();
        }

        activeObject.SetActive(true);

        PSOConfig config = activeObject.GetComponent<PSOConfig>();
        config.seed = seed;

        PSOCameraController controller = FindObjectOfType<PSOCameraController>();
        if (controller)
        {
            controller.SetSeed(seed);
        }

        PSORender psoRender = activeObject.GetComponent<PSORender>();
        psoRender.playSpeed = speed;

        if (isCustom)
        {
            psoRender.displayConnectivity = connectivity;

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
            psoRender.fogOfFunction = fof;
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
            randomPreset.invertImage = true;

            psoRender.yScale = -rnd.Gaussian(20.0f, 4.0f);
            psoRender.materialOverride = materialsImage[rnd.Range(0, materialsImage.Length)];
        }

        psoRender.fogOfFunction = (rnd.Range(0, 100) < 50);
    }

    public static List<string> ParseText(string line, Char delimiter, Char textQualifier)
    {
        List<string> ret = new List<string>();

        string currentToken = "";
        bool   inString = false;

        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == textQualifier)
            {
                inString = !inString;
                currentToken += line[i];
            }
            else if ((line[i] == delimiter) && (!inString))
            {
                ret.Add(currentToken);
                currentToken = "";
            }
            else
            {
                currentToken += line[i];
            }            
        }

        if (currentToken.Length > 0)
        {
            ret.Add(currentToken);
        }

        return ret;
    }
}
