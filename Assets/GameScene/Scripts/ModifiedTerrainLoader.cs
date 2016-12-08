using UnityEngine;
using System.Collections;

public class ModifiedTerrainLoader : MonoBehaviour {

    public Terrain Terrain;

    public HeightmapGenerator HeightmapGenerator;
    public TextureAlphaGenerator TextureAlphaGenerator;

    void Start()
    {
        GenerateInitial(Terrain.terrainData);
    }

    private void GenerateInitial(TerrainData td)
    {
        int width = td.heightmapWidth;
        int height = td.heightmapHeight;
        int max = (int)td.size.y;

        float[,] heightmap = HeightmapGenerator.GenerateHeightMap(width, height, max);
        float[,,] alphaMap = TextureAlphaGenerator.GenerateTextureAlphas(heightmap, max, td.alphamapWidth, td.alphamapHeight);
        td.SetHeights(0, 0, heightmap);
        td.SetAlphamaps(0, 0, alphaMap);
    }
}
