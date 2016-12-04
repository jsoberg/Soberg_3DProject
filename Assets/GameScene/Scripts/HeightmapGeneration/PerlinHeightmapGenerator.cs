using UnityEngine;
using System.Collections;

public class PerlinHeightmapGenerator : HeightmapGenerator
{
    public float Factor;

    public override float[,] GenerateHeightMap(int width, int height, int max)
    {
        float[,] heightMap = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                float perlinX = ((float)x / (float) width) * Factor;
                float perlinY = ((float)y / (float) height) * Factor;
                float noise = Mathf.PerlinNoise(perlinX, perlinY);
                heightMap[x, y] = noise;
            }
        }

        return heightMap;
    }
}
