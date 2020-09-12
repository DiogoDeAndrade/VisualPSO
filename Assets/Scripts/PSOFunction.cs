using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSOFunction : MonoBehaviour
{
    public Vector2Int nPoints;
    public Rect       extents;
    public float      density;
    public float      minY, maxY;

    protected float[,] functionValues;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Parse(TextAsset data, float yScale = 1.0f)
    {
        var splitFile = new string[] { "\r\n", "\r", "\n" };
        var splitLine = new string[] { " " };
        var lines = data.text.Split(splitFile, System.StringSplitOptions.RemoveEmptyEntries);

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
