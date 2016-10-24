using UnityEngine;
using System.Collections;

public class TerrainHeightmapGenerator : MonoBehaviour
{

    void Start ()
    {
        GenerateHeightmap(UnityEngine.Random.Range(0f, 1f));
    }

    private void GenerateHeightmap(float effector)
    {
        TerrainData TD = GetComponent<Terrain>().terrainData;
        float[,] HeightMap = new float[TD.heightmapWidth, TD.heightmapWidth];

        for (int x = 0; x < TD.heightmapWidth; x++)
        {
            for (int y = 0; y < TD.heightmapHeight; y++)
            {
                HeightMap[x, y] = Mathf.PerlinNoise((float) x / (float) TD.heightmapWidth, (float) y / (float)TD.heightmapHeight);
            }
        }

        TD.SetHeights(0, 0, HeightMap);
    }

    private Vector3 CurrentMainCamTransform;

	void Update ()
    {

    }
}
