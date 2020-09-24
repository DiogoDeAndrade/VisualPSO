using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSOFunction : MonoBehaviour
{
    public Vector2Int nPoints;
    public Rect       extents;
    public float      density;
    public float      minY, maxY;
    public PSORender  manager;

    protected float[,] functionValues;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    virtual public void SetTexture(Texture2D texture)
    {

    }

    public void Parse(string data, float yScale = 1.0f)
    {
        var splitFile = new string[] { "\r\n", "\r", "\n" };
        var splitLine = new string[] { " " };
        var lines = data.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);

        nPoints = new Vector2Int(int.Parse(lines[0]), int.Parse(lines[1]));
        float x1 = float.Parse(lines[2]);
        float y1 = float.Parse(lines[3]);
        float x2 = float.Parse(lines[4]);
        float y2 = float.Parse(lines[5]);
        extents = new Rect(x1, y1, x2 - x1, y2 - y1);
        density = float.Parse(lines[6]);

        functionValues = new float[nPoints.y, nPoints.x];

        minY = float.MaxValue;
        maxY = -float.MaxValue;

        for (int y = 7; y < lines.Length; y++)
        {
            var vals = lines[y].Split(splitLine, System.StringSplitOptions.None);

            for (int x = 0; x < nPoints.x; x++)
            {
                float v = float.Parse(vals[x]) * yScale;
                functionValues[y - 7, x] = v;
                minY = Mathf.Min(v, minY);
                maxY = Mathf.Max(v, maxY);
            }
        }

        OnDataProcessed();
    }

    public void Parse(OpenPSO.Lib.IFunction function, int nSamples, Vector2 samplingInterval, float yScale = 1.0f)
    {
        nPoints = new Vector2Int(nSamples, nSamples);
        functionValues = new float[nSamples, nSamples];

        float x1 = float.MaxValue;
        float x2 = -float.MaxValue;
        float y1 = float.MaxValue;
        float y2 = -float.MaxValue;

        minY = float.MaxValue;
        maxY = -float.MaxValue;

        for (int y = 0; y < nSamples; y++)
        {
            double py = samplingInterval.x + (samplingInterval.y - samplingInterval.x) * ((double)y / nSamples);

            y1 = Mathf.Min((float)py, y1);
            y2 = Mathf.Max((float)py, y2);

            for (int x = 0; x < nSamples; x++)
            {
                double px = samplingInterval.x + (samplingInterval.y - samplingInterval.x) * ((double)x / nSamples);

                float v = (float)function.Evaluate(new List<double> { px, py }) * yScale;
                functionValues[y, x] = v;
                minY = Mathf.Min(v, minY);
                maxY = Mathf.Max(v, maxY);
                x1 = Mathf.Min((float)px, x1);
                x2 = Mathf.Max((float)px, x2);
            }
        }

        extents = new Rect(x1, y1, x2 - x1, y2 - y1);

        OnDataProcessed();
    }

    virtual protected void OnDataProcessed()
    {

    }

    protected void NormalizeData()
    {
        for (int y = 0; y < nPoints.y; y++)
        {
            for (int x = 0; x < nPoints.x; x++)
            {
                functionValues[y, x] = (functionValues[y, x] - minY) / (maxY - minY);
            }
        }
    }

    protected void FlipZ()
    {
        var flippedFunction = new float[nPoints.y, nPoints.x];

        for (int y = 0; y < nPoints.y; y++)
        {
            for (int x = 0; x < nPoints.x; x++)
            {
                flippedFunction[y, x] = functionValues[nPoints.y - y - 1, x];
            }
        }

        functionValues = flippedFunction;
    }
}
