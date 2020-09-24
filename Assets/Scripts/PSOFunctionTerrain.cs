using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class PSOFunctionTerrain : PSOFunction
{
    public bool     terrainGrow = false;
    [ShowIf("terrainGrow")]
    public float    growthSpeed = 1.0f;
    [ShowIf("terrainGrow")]
    public bool     variableSpeed = false;
    public Terrain  terrain;
    public Terrain  ghostTerrain;

    float[,]    mask;
    float[,]    growSpeedMask;
    float[,]    actualValues;

    override public void SetTexture(Texture2D texture)
    {
        if (terrain)
        {
            terrain.materialTemplate.SetTexture("Image", texture);
        }
    }

    override protected void OnDataProcessed()
    {
        NormalizeData();
        terrain.terrainData.size = new Vector3(extents.width, (maxY - minY), extents.height);
        terrain.terrainData.heightmapResolution = nPoints.x;
        terrain.terrainData.SetHeights(0, 0, functionValues);

        transform.position = new Vector3(extents.x, minY, extents.y);

        if (ghostTerrain)
        {
            ghostTerrain.terrainData.size = new Vector3(extents.width, (maxY - minY), extents.height);
            ghostTerrain.terrainData.heightmapResolution = nPoints.x;
            ghostTerrain.terrainData.SetHeights(0, 0, functionValues);

            ghostTerrain.transform.position = new Vector3(extents.x, minY, extents.y);
        }

        if (terrainGrow)
        {
            actualValues = functionValues;
            mask = new float[nPoints.y, nPoints.x];
            functionValues = new float[nPoints.y, nPoints.x];

            UpdateMask(true);
            UpdateValues();

            if (variableSpeed)
            {
                growSpeedMask = new float[nPoints.y, nPoints.x];
            }
        }
    }

    void UpdateMask(bool forceStartup)
    {
        // Growth
        if (growSpeedMask != null)
        {
            for (int y = 0; y < nPoints.y; y++)
            {
                for (int x = 0; x < nPoints.x; x++)
                {
                    if ((mask[y, x] > 0) && (mask[y, x] < 1.0f))
                    {
                        mask[y, x] = Mathf.Clamp01(mask[y, x] + Time.deltaTime * growSpeedMask[y,x] * growthSpeed);
                    }
                }
            }
        }
        else
        {
            for (int y = 0; y < nPoints.y; y++)
            {
                for (int x = 0; x < nPoints.x; x++)
                {
                    if ((mask[y, x] > 0) && (mask[y, x] < 1.0f))
                    {
                        mask[y, x] = Mathf.Clamp01(mask[y, x] + Time.deltaTime * growthSpeed);
                    }
                }
            }
        }

        if (manager)
        {
            var particles = manager.GetParticles();
            if (forceStartup)
            {
                foreach (var p in particles)
                {
                    Vector3 pos = p.transform.position;
                    int px = Mathf.RoundToInt(nPoints.x * (pos.x - terrain.transform.position.x) / extents.width);
                    int pz = Mathf.RoundToInt(nPoints.y * (pos.z - terrain.transform.position.z) / extents.height);

                    PaintMask(px, pz, 5, 1.0f);
                }
            }
            else
            {
                foreach (var p in particles)
                {
                    Vector3 pos = p.transform.position;
                    int px = Mathf.RoundToInt(nPoints.x * (pos.x - terrain.transform.position.x) / extents.width);
                    int pz = Mathf.RoundToInt(nPoints.y * (pos.z - terrain.transform.position.z) / extents.height);

                    PaintMask(px, pz, 5, Time.deltaTime);
                }
            }
        }
    }

    void PaintMask(int x, int y, float value)
    {
        mask[y, x] = Mathf.Max(mask[y, x], value);

        if (y > 0) mask[y - 1, x] = Mathf.Max(mask[y - 1, x], value * 0.5f);
        if (y < nPoints.y - 1) mask[y + 1, x] = Mathf.Max(mask[y + 1, x], value * 0.5f);
        if (x > 0) mask[y, x - 1] = Mathf.Max(mask[y, x - 1], value * 0.5f);
        if (x < nPoints.x - 1) mask[y, x + 1] = Mathf.Max(mask[y, x + 1], value * 0.5f);
    }

    void PaintMask(int x, int y, int radius, float value)
    {
        mask[y, x] = Mathf.Max(mask[y, x], value);
        if (growSpeedMask != null) growSpeedMask[y, x] = 1.0f;

        for (int dy = -radius; dy <= radius; dy++)
        {
            int yy = y + dy;

            if (yy < 0) continue;
            if (yy >= nPoints.y) break;

            for (int dx = -radius; dx <= radius; dx++)
            {
                int xx = x + dx;

                if (xx < 0) continue;
                if (xx >= nPoints.x) break;

                float s = (radius - Mathf.Sqrt(dx * dx + dy * dy)) / radius;

                mask[yy, xx] = Mathf.Max(mask[yy, xx], value * s);
                if (growSpeedMask != null) growSpeedMask[yy, xx] = Mathf.Max(growSpeedMask[yy, xx], Mathf.Pow(s, 3.0f));
            }
        }
    }

    void UpdateValues()
    {
        for (int y = 0; y < nPoints.y; y++)
        {
            for (int x = 0; x < nPoints.x; x++)
            {
                functionValues[y, x] = actualValues[y, x] * mask[y, x];
            }
        }

        terrain.terrainData.SetHeights(0, 0, functionValues);
    }

    private void LateUpdate()
    {
        if (terrainGrow)
        {
            UpdateMask(false);
            UpdateValues();
        }
    }
}
