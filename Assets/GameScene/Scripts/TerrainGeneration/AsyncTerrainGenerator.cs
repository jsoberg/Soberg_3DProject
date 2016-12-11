using UnityEngine;
using System.Collections;
using System.Threading;

public class AsyncTerrainGenerator
{
    public readonly HeightmapGenerator HeightmapGenerator;
    public readonly TextureAlphaGenerator TextureAlphaGenerator;

    public bool AtomicIsLoadingNextChunkData = false;

    public float[,] NextHeightmap = null;
    public float[,,] NextAlphaMap = null;

    public AsyncTerrainGenerator(HeightmapGenerator heightmapGenerator, TextureAlphaGenerator textureGenerator)
    {
        this.HeightmapGenerator = heightmapGenerator;
        this.TextureAlphaGenerator = textureGenerator;
    }

    public void LoadNextChunkDataAsync(int width, int height, int max, int alphamapWidth, int alphamapHeight)
    {
        // Immediately signal that we are loading the next heightmap.
        AtomicIsLoadingNextChunkData = true;

        var thread = new Thread(() => LoadNextChunkData(width, height, max, alphamapWidth, alphamapHeight));
        thread.Start();
    }

    private void LoadNextChunkData(int width, int height, int max, int alphamapWidth, int alphamapHeight)
    {
        AtomicIsLoadingNextChunkData = true;

        NextHeightmap = null;
        NextAlphaMap = null;

        NextHeightmap = HeightmapGenerator.GenerateHeightMap(width, height, max);
        NextAlphaMap = TextureAlphaGenerator.GenerateTextureAlphas(NextHeightmap, max, alphamapWidth, alphamapHeight);

        // We're done loading.
        AtomicIsLoadingNextChunkData = false;
    }
}
