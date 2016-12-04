using UnityEngine;
using System.Collections;
using System.Linq;

public class MountainSideSplatMapGenerator : TextureAlphaGenerator
{
    public float SandHeight;

    public override float[,,] GenerateTextureAlphas(float[,] heightmap, float maxHeight, int alphamapWidth, int alphamapHeight)
    { 
        float[,,] splatmapData = new float[alphamapWidth, alphamapHeight, 3];
        float heightmapHeight = heightmap.GetLength(0);
        float heightmapWidth = heightmap.GetLength(1);

        for (int y = 0; y < alphamapHeight; y++)
        {
            for (int x = 0; x < alphamapWidth; x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float) y / (float) alphamapHeight;
                float x_01 = (float) x / (float) alphamapWidth;

                float height = heightmap[Mathf.RoundToInt(x_01 * heightmapWidth), Mathf.RoundToInt(y_01 * heightmapHeight)] * maxHeight;
                Vector3 normal = GetInterpolatedNormal(heightmap, x, y, maxHeight);
                float steepness = Mathf.Acos(Vector3.Dot(normal, Vector3.up));
                float[] splatWeights = new float[3];

                if (height > SandHeight) {
                    // Texture[0] stronger on flatter terrain
                    splatWeights[0] = Mathf.Abs(0.5f - Mathf.Clamp01(steepness * steepness / (heightmapHeight / 5.0f)));
                    // Texture[1] increases with height but only on surfaces facing positive Z axis 
                    splatWeights[1] = (height / 8) * Mathf.Clamp01(normal.z);
                } else {
                    // Texture[2] is used for sand.
                    splatWeights[2] = 1f;
                }

                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int i = 0; i < 3; i++)
                {
                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }

        return splatmapData;
    }

    private Vector3 GetInterpolatedNormal(float[,] Heightmap, int x, int y, float maxHeight)
    {
        float slopeX = Heightmap[x < Heightmap.Length - 1 ? x + 1 : x, y] - Heightmap[x > 0 ? x - 1 : x, y];
        float slopeZ = Heightmap[x, y < Heightmap.Length - 1 ? y + 1 : y] - Heightmap[x, y > 0 ? y - 1 : y];

        if (x == 0 || x == Heightmap.Length - 1)
            slopeX *= 2;
        if (y == 0 || y == Heightmap.Length - 1)
            slopeZ *= 2;

        slopeX *= maxHeight;
        slopeZ *= maxHeight;

        Vector3 normal = new Vector3(-slopeX, 2, slopeZ);
        normal.Normalize();
        return normal;
    }
}