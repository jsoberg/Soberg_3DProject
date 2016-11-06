using UnityEngine;
using System.Collections;
using System.Threading;

public class TerrainChunkLoader : MonoBehaviour
{
    public Terrain FrontTerrain;
    public Terrain BackTerrain;

    public HeightmapGenerator HeightmapGenerator;
    public int ThreshholdForChunkLoad = 1024;

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
        LoadNextHeightmapAsync(width, height, max);
    }

    private void GenerateInitial(TerrainData td)
    {
        int width = td.heightmapWidth; 
        int height = td.heightmapHeight;
        int max = (int) td.size.y;

        float[,] heightmap = HeightmapGenerator.GenerateHeightMap(width, height, max);
        td.SetHeights(0,0, heightmap);
    }

	void Update ()
    {
        Vector3 camPosition = Camera.main.transform.position;
        if ((CurrentChunkEndX - camPosition.x) < ThreshholdForChunkLoad) {
            MoveBackTerrainUpAndSwap();

            TerrainData loadedTerrain = FrontTerrain.terrainData;
            float[,] heightmap = WaitForNextHeightmap();
            loadedTerrain.SetHeights(0,0, heightmap);
            LoadNextHeightmapAsync(
                loadedTerrain.heightmapWidth, loadedTerrain.heightmapHeight, (int)loadedTerrain.size.y);
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

    private float[,] WaitForNextHeightmap()
    {
        while (AtomicIsLoadingNextHeightmap) {
            Thread.Sleep(1);
        }
        return NextHeightmap;
    }

    private bool AtomicIsLoadingNextHeightmap = false;
    private float[,] NextHeightmap = null;

    private void LoadNextHeightmapAsync(int width, int height, int max)
    {
        var thread = new Thread(() => LoadNextHeightmap(width, height, max));
        thread.Start();
    }

    private void LoadNextHeightmap(int width, int height, int max)
    {
        // Signal that we are loading the next heightmap.
        AtomicIsLoadingNextHeightmap = true;

        NextHeightmap = null;
        NextHeightmap = HeightmapGenerator.GenerateHeightMap(width, height, max);

        // We're done loading.
        AtomicIsLoadingNextHeightmap = false;
    }
}
