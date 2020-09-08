using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSOFunctionTerrain : PSOFunction
{
    override protected void OnDataProcessed()
    {
        Terrain terrain = GetComponent<Terrain>();

        NormalizeData();
        terrain.terrainData.size = new Vector3(extents.width, (maxY - minY), extents.height);
        terrain.terrainData.heightmapResolution = nPoints.x;
        terrain.terrainData.SetHeights(0, 0, functionValues);

        transform.position = new Vector3(extents.x, minY, extents.y);
    }

}
