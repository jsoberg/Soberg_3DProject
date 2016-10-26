using UnityEngine;
using System.Collections;

public class TerrainHeightmapGenerator : MonoBehaviour
{
    void Start ()
    {
        GenerateHeightmap();
    }

    private void GenerateHeightmap()
    {
        TerrainData TD = GetComponent<Terrain>().terrainData;
        float[,] HeightMap = new float[TD.heightmapWidth, TD.heightmapWidth];

        for (int x = 0; x < TD.heightmapWidth; x++)
        {
            for (int y = 0; y < TD.heightmapHeight; y++)
            {
                float perlinX = ((float)x / (float)TD.heightmapWidth) * 10f;
                float perlinY = ((float)y / (float)TD.heightmapHeight) * 10f;
                float noise = Mathf.PerlinNoise(perlinX, perlinY);
                HeightMap[x, y] = noise;
            }
        }

        TD.SetHeights(0, 0, HeightMap);
    }

	void Update ()
    {

    }
}
