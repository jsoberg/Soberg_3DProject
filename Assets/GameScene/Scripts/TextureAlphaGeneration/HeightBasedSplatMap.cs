using UnityEngine;
using System.Collections;
using System.Linq;

public class HeightBasedSplatMap : TextureAlphaGenerator
{
    public float PercentageOfTextureBasedOnHeight = .8f;

    public override void RegenerateTextureAlphas()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;

        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        float min, max;
        FindMinAndMaxHeight(out min, out max);

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float) y / (float) terrainData.alphamapHeight;
                float x_01 = (float) x / (float) terrainData.alphamapWidth;

                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight), Mathf.RoundToInt(x_01 * terrainData.heightmapWidth));

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.alphamapLayers];
                // Find the splatmap to show given the current height
                int splatMapToShow = GetSplatMapToShow(min, max, height, terrainData.alphamapLayers);

                // First splat (black) is constant.
                splatWeights[0] = .2f;
                for (int i = 1; i < terrainData.alphamapLayers; i ++) {
                    splatWeights[i] = (i == splatMapToShow) ? PercentageOfTextureBasedOnHeight : ((1 - PercentageOfTextureBasedOnHeight) / (terrainData.alphamapLayers - 1));
                }

                float z = splatWeights.Sum();
                // Loop through each terrain texture
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;
                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }

        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    int GetSplatMapToShow(float minHeight, float maxHeight, float height, int numLayers)
    {
        float diff = maxHeight - minHeight;
        float groupSize = diff / numLayers;
        return (int)Mathf.Round(height/groupSize) - 1;
    }

    void FindMinAndMaxHeight(out float min, out float max)
    {
        TerrainData td = GetComponent<Terrain>().terrainData;

        min = float.MaxValue;
        max = float.MinValue;
        for (int x = 0; x < td.heightmapWidth; x++)
        {
            for (int y = 0; y < td.heightmapHeight; y++)
            {
                float height = td.GetHeight(x, y);
                if (height < min)
                    min = height;
                if (height > max)
                    max = height;
            }
        }
    }

    // Add some random "noise" to the alphamaps.
    void AddAlphaNoise(Terrain t, float noiseScale)
    {
        float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);

        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {
                float a0 = maps[x, y, 0];
                float a1 = maps[x, y, 1];

                a0 += Random.value * noiseScale;
                a1 += Random.value * noiseScale;

                float total = a0 + a1;

                maps[x, y, 0] = a0 / total;
                maps[x, y, 1] = a1 / total;
            }
        }

        t.terrainData.SetAlphamaps(0, 0, maps);
    }
}
