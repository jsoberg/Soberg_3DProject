using UnityEngine;
using System.Collections;
using System.Linq;

public class MidpointDisplacementTerrainHeightmapGenerator : HeightmapGenerator
{
    public float Seed = 0f;
    public float Roughness = .6f;

    private float[,] LastHeightMap;

    public override float[,] GenerateHeightMap(int width, int height, int max)
    {
        float[,] heightMap = new float[width, height];
        SeedHeightmap(heightMap, width, height);
        BlendHeightmapEdges(heightMap, width, height);

        Divide(height, heightMap, width, height, max);
        NormalizeHeightmap(heightMap);

        LastHeightMap = heightMap;
        return heightMap;
    }

    private void BlendHeightmapEdges(float[,] heightMap, int width, int height)
    {
        if (LastHeightMap == null) {
            return;
        }

        for (int i = 0; i < width; i++) {
            heightMap[i, (height - 1)] = LastHeightMap[i, 0];
        }
    }

    private void SeedHeightmap(float[,] heightMap, int width, int height)
    {
        heightMap[0, 0] = Seed;
        heightMap[width - 1, 0] = Seed;
        heightMap[width - 1, height - 1] = Seed;
        heightMap[0, height - 1] = Seed;
    }

    private void Divide(int size, float[,] heightMap, int width, int height, int max)
    {
        int x, y, half = (size / 2);
        float scale = (Roughness * size);
        if (half < 1) return;

        System.Random random = new System.Random();

        for (y = half; y < height; y += size)
        {
            for (x = half; x < width; x += size)
            {
                float offset = ((float) random.NextDouble() * scale * 2 - scale) / max;
                Square(x, y, width, height, max, half, offset, heightMap);
            }
        }
        for (y = 0; y < height; y += half)
        {
            for (x = (y + half) % size; x < width; x += size)
            {
                float offset = ((float) random.NextDouble() * scale * 2 - scale) / max;
                Diamond(x, y, width, height, max, half, offset, heightMap);
            }
        }
        Divide((size / 2), heightMap, width, height, max);
    }

    private void Square(int x, int y, int width, int height, int max, int size, float offset, float[,] heightMap)
    {
        if (y == 0 || y == heightMap.GetLength(1) - 1) {
            return;
        }

        float[] corners = {
         heightMap[Mathf.Max(0, x - size), Mathf.Max(0, y - size)],                   // upper left corner
         heightMap[Mathf.Min(width - 1, x + size), Mathf.Max(0, y - size)],           // upper right corner
         heightMap[Mathf.Min(width - 1, x + size), Mathf.Min(height - 1, y + size)],  // lower right corner
         heightMap[Mathf.Max(0, x - size), Mathf.Min(height - 1, y + size)]};         // lower left corner

        heightMap[x, y] = (corners.Average() + offset) <= max ? (corners.Average() + offset) : (corners.Average() - offset);
    }

    private void Diamond(int x, int y, int width, int height, int max, int size, float offset, float[,] heightMap)
    {
        if (y == 0 || y == heightMap.GetLength(1) - 1) {
            return;
        }

        float[] edges = {
          heightMap[x, Mathf.Max(0, y - size)],               // top edge
          heightMap[Mathf.Min(width - 1, x + size), y],       // right edge
          heightMap[x, Mathf.Min(height - 1, y + size)],      // bottom edge
          heightMap[Mathf.Max(0, x - size), y] };             // left edge

        heightMap[x, y] = (edges.Average() + offset) <= max ? (edges.Average() + offset) : (edges.Average() - offset);
    }

    private void NormalizeHeightmap(float[,] heightMap)
    {
        for (int i = 0; i < heightMap.GetLength(0); i ++)
        {
            for (int j = 0; j < heightMap.GetLength(0); j++)
            {
                float val = heightMap[i, j];
                val = Mathf.Max(val, 0);
                val = Mathf.Min(val, 1);
                heightMap[i, j] = val;
            }
        }
    }
}
