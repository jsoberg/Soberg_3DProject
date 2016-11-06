using UnityEngine;
using System.Collections;
using System.Linq;

public class MidpointDisplacementTerrainHeightmapGenerator : HeightmapGenerator
{
    public readonly float Seed = 400f;
    public readonly float Roughness = .6f;

    public override float[,] GenerateHeightMap(int width, int height, int max)
    {
        float[,] heightMap = new float[width, height];
        Divide(height, heightMap, width, height, max);
        return heightMap;
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
        float[] corners = {
         heightMap[Mathf.Max(0, x - size), Mathf.Max(0, y - size)],                   // upper left corner
         heightMap[Mathf.Min(width - 1, x + size), Mathf.Max(0, y - size)],           // upper right corner
         heightMap[Mathf.Min(width - 1, x + size), Mathf.Min(height - 1, y + size)],  // lower right corner
         heightMap[Mathf.Max(0, x - size), Mathf.Min(height - 1, y + size)]};         // lower left corner

        heightMap[x, y] = (corners.Average() + offset) <= max ? (corners.Average() + offset) : (corners.Average() - offset);
    }

    private void Diamond(int x, int y, int width, int height, int max, int size, float offset, float[,] heightMap)
    {
        float[] edges = {
          heightMap[x, Mathf.Max(0, y - size)],               // top edge
          heightMap[Mathf.Min(width - 1, x + size), y],       // right edge
          heightMap[x, Mathf.Min(height - 1, y + size)],      // bottom edge
          heightMap[Mathf.Max(0, x - size), y] };             // left edge

        heightMap[x, y] = (edges.Average() + offset) <= max ? (edges.Average() + offset) : (edges.Average() - offset);
    }
}
