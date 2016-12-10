using UnityEngine;
using System.Collections;
using System.Threading;

public class TerrainChunkLoader : MonoBehaviour
{
    public Terrain TerrainPrefab;
    public TerrainData TerrainDataPrefab;
    public GameObject Water;

    public HeightmapGenerator HeightmapGenerator;
    public TextureAlphaGenerator TextureAlphaGenerator;
    public int ThreshholdForChunkLoad;

    private float CurrentChunkEndX = 0;

    void Start()
    {
        CurrentChunkEndX += 4096;

        // Start loading up for when we load later on.
        TerrainData initialTd = GenerateNewTerrainWithHeightmaps().terrainData;
        int width = initialTd.heightmapWidth;
        int height = initialTd.heightmapHeight;
        int max = (int) initialTd.size.y;
        LoadNextChunkDataAsync(width, height, max, initialTd.alphamapWidth, initialTd.alphamapHeight);
    }

    private Terrain GenerateNewTerrainWithHeightmaps()
    {
        Terrain newTerrainChunk = GenerateNewTerrain();
        TerrainData td = newTerrainChunk.terrainData;

        int width = td.heightmapWidth; 
        int height = td.heightmapHeight;
        int max = (int) td.size.y;

        float[,] heightmap = HeightmapGenerator.GenerateHeightMap(width, height, max);
        float[,,] alphaMap = TextureAlphaGenerator.GenerateTextureAlphas(heightmap, max, td.alphamapWidth, td.alphamapHeight);
        td.SetHeights(0,0, heightmap);
        td.SetAlphamaps(0,0, alphaMap);

        return newTerrainChunk;
    }

    private Terrain GenerateNewTerrain()
    {
        Terrain newTerrainChunk = Object.Instantiate(TerrainPrefab);
        TerrainData td = Object.Instantiate(TerrainDataPrefab);
        newTerrainChunk.terrainData = td;
        return newTerrainChunk;
    }

    void Update()
    {
        Vector3 camPosition = Camera.main.transform.position;
        if ((CurrentChunkEndX - camPosition.x) < ThreshholdForChunkLoad)
        {
            Terrain loadedTerrain = GenerateNewTerrain();
            TerrainData loadedTerrainData = loadedTerrain.terrainData;
            WaitForNextChunkData();
            loadedTerrainData.SetHeights(0, 0, NextHeightmap);
            loadedTerrainData.SetAlphamaps(0, 0, NextAlphaMap);
            loadedTerrain.transform.position += new Vector3(CurrentChunkEndX, 0, 0);

            CurrentChunkEndX += 4096;

            LoadNextChunkDataAsync(
                loadedTerrainData.heightmapWidth, loadedTerrainData.heightmapHeight, (int)loadedTerrainData.size.y, loadedTerrainData.alphamapWidth, loadedTerrainData.alphamapHeight);
        }
    }
	

    private void WaitForNextChunkData()
    {
        while (AtomicIsLoadingNextChunkData) {
            Thread.Sleep(1);
        }
    }

    private bool AtomicIsLoadingNextChunkData = false;
    private float[,] NextHeightmap = null;
    private float[,,] NextAlphaMap = null;

    private void LoadNextChunkDataAsync(int width, int height, int max, int alphamapWidth, int alphamapHeight)
    {
        var thread = new Thread(() => LoadNextChunkData(width, height, max, alphamapWidth, alphamapHeight));
        thread.Start();
    }

    private void LoadNextChunkData(int width, int height, int max, int alphamapWidth, int alphamapHeight)
    {
        // Signal that we are loading the next heightmap.
        AtomicIsLoadingNextChunkData = true;

        NextHeightmap = null;
        NextAlphaMap = null;

        NextHeightmap = HeightmapGenerator.GenerateHeightMap(width, height, max);
        NextAlphaMap = TextureAlphaGenerator.GenerateTextureAlphas(NextHeightmap, max, alphamapWidth, alphamapHeight);

        // We're done loading.
        AtomicIsLoadingNextChunkData = false;
    }
}
