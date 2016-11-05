using UnityEngine;
using System.Collections;
using System.Linq;

public class MidpointDisplacementTerrainHeightmapGenerator : MonoBehaviour
{
    public readonly float Seed = 400f;
    public readonly float Roughness = .2f;

	void Start ()
    {
        Generate();
	}
	
    void Generate()
    {
        TerrainData td = GetComponent<Terrain>().terrainData;
        float[,] heightMap = new float[td.heightmapWidth, td.heightmapHeight];

        heightMap[0, 0] = Seed / td.size.y;
        heightMap[td.heightmapWidth - 1, 0] = Seed / td.size.y;
        heightMap[td.heightmapWidth - 1, td.heightmapHeight - 1] = Seed / td.size.y;
        heightMap[0, td.heightmapHeight - 1] = Seed / td.size.y;

        Divide(td.heightmapHeight - 1, heightMap);
        td.SetHeights(0, 0, heightMap);
    }

    private void Divide(int size, float[,] heightMap)
    {
        TerrainData td = GetComponent<Terrain>().terrainData;

        int x, y, half = (size / 2);
        float scale = (Roughness * size);
        if (half < 1) return;

        for (y = half; y < td.heightmapHeight; y += size)
        {
            for (x = half; x < td.heightmapWidth; x += size)
            {
                float offset = (Random.Range(0f, 1f) * scale * 2 - scale) / td.size.y;
                Square(x, y, half, offset, heightMap, td);
            }
        }
        for (y = 0; y < td.heightmapHeight; y += half)
        {
            for (x = (y + half) % size; x < td.heightmapWidth; x += size)
            {
                float offset = (Random.Range(0f, 1f) * scale * 2 - scale) / td.size.y;
                Diamond(x, y, half, offset, heightMap, td);
            }
        }
        Divide((size / 2), heightMap);
    }

    private void Square(int x, int y, int size, float offset, float[,] heightMap, TerrainData td)
    {
        float[] corners = {
         heightMap[x - size, y - size],     // upper left corner
         heightMap[x + size, y - size],     // upper right corner
         heightMap[x + size, y + size],     // lower right corner
         heightMap[x - size, y + size]};    // lower left corner

        heightMap[x, y] = (corners.Average() + offset) <= td.size.y ? (corners.Average() + offset) : (corners.Average() - offset);
    }

    private void Diamond(int x, int y, int size, float offset, float[,] heightMap, TerrainData td)
    {
        float[] edges = {
          heightMap[x, Mathf.Max(0, y - size)],                           // top edge
          heightMap[Mathf.Min(td.heightmapWidth - 1, x + size), y],       // right edge
          heightMap[x, Mathf.Min(td.heightmapHeight - 1, y + size)],      // bottom edge
          heightMap[Mathf.Max(0, x - size), y] };                         // left edge

        heightMap[x, y] = (edges.Average() + offset) <= td.size.y ? (edges.Average() + offset) : (edges.Average() - offset);
    }
}
