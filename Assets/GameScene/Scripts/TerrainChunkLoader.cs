using UnityEngine;
using System.Collections;
using System.Threading;

public class TerrainChunkLoader : MonoBehaviour
{
    public Terrain FrontTerrain;
    public Terrain BackTerrain;

    public HeightmapGenerator HeightmapGenerator;
    public TextureAlphaGenerator TextureAlphaGenerator;
    public int ThreshholdForChunkLoad = 2 ^ 15;

    private float CurrentChunkEndX = 0;

    void Start()
    {
        GenerateInitial(BackTerrain.terrainData);
        GenerateInitial(FrontTerrain.terrainData);
        CurrentChunkEndX += FrontTerrain.terrainData.size.x * 2;

        // Start loading up for when we swap later on.
        TerrainData frontTd = FrontTerrain.terrainData;
        int width = frontTd.heightmapWidth;
        int height = frontTd.heightmapHeight;
        int max = (int)frontTd.size.y;
        LoadNextChunkDataAsync(width, height, max, frontTd.alphamapWidth, frontTd.alphamapHeight);
    }

    private void GenerateInitial(TerrainData td)
    {
        int width = td.heightmapWidth; 
        int height = td.heightmapHeight;
        int max = (int) td.size.y;

        float[,] heightmap = HeightmapGenerator.GenerateHeightMap(width, height, max);
        float[,,] alphaMap = TextureAlphaGenerator.GenerateTextureAlphas(heightmap, max, td.alphamapWidth, td.alphamapHeight);
        td.SetHeights(0,0, heightmap);
        td.SetAlphamaps(0,0, alphaMap);
    }

	void Update ()
    {
        Vector3 camPosition = Camera.main.transform.position;
        if ((CurrentChunkEndX - camPosition.x) < ThreshholdForChunkLoad) {
            MoveBackTerrainUpAndSwap();

            TerrainData loadedTerrain = FrontTerrain.terrainData;
            WaitForNextChunkData();
            loadedTerrain.SetHeights(0,0, NextHeightmap);
            loadedTerrain.SetAlphamaps(0, 0, NextAlphaMap);

            LoadNextChunkDataAsync(
                loadedTerrain.heightmapWidth, loadedTerrain.heightmapHeight, (int)loadedTerrain.size.y, loadedTerrain.alphamapWidth, loadedTerrain.alphamapHeight);
        }
	}

    private void MoveBackTerrainUpAndSwap()
    {
        float movement = BackTerrain.terrainData.size.x * 2;
        Vector3 moveTranslation = new Vector3(movement, 0, 0);
        BackTerrain.transform.Translate(moveTranslation);

        Terrain oldFront = FrontTerrain;
        FrontTerrain = BackTerrain;
        BackTerrain = oldFront;

        BackTerrain.SetNeighbors(null, FrontTerrain, null, null);
        FrontTerrain.SetNeighbors(null, null, null, BackTerrain);

        CurrentChunkEndX += movement / 2;
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
