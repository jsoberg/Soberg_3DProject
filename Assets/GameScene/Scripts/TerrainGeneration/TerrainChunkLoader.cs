using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainChunkLoader : MonoBehaviour
{
    public Terrain TerrainPrefab;
    public TerrainData TerrainDataPrefab;
    public GameObject Water;

    public HeightmapGenerator HeightmapGenerator;
    public TextureAlphaGenerator TextureAlphaGenerator;
    public int ThreshholdForChunkLoad;

    private readonly Stack<TerrainChunkInfo> ChunkInfoStack = new Stack<TerrainChunkInfo>();

    void Start()
    {
        Terrain initialTerrain = GenerateNewTerrainWithHeightmaps();
        TerrainData initialTd = initialTerrain.terrainData;
        ChunkInfoStack.Push(new TerrainChunkInfo(initialTerrain.transform.position, (int)initialTd.size.x, (int)initialTd.size.z));
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
        if (!DoesChunkExistForPosition(camPosition))
        {
            StartCoroutine(GenerateNextChunkCoroutine(camPosition));
        }
    }

    private bool DoesChunkExistForPosition(Vector3 position)
    {
        foreach (TerrainChunkInfo info in ChunkInfoStack)
        {
            int endX = info.Width + (int) info.Position.x;
            int endZ = info.Height + (int) info.Position.z;
            if (isWithinParameters(position.x, info.Position.x, endX) && isWithinParameters(position.z, info.Position.z, endZ))
            {
                return true;
            }
        }
        return false;
    }

    private bool isWithinParameters(float value, float startParam, float endParam)
    {
        return (value < endParam && value > startParam);
    }

    private IEnumerator GenerateNextChunkCoroutine(Vector3 positionToGenerateFor)
    {
        Terrain loadedTerrain = GenerateNewTerrain();
        TerrainData td = loadedTerrain.terrainData;
        int terrainWidth = (int) td.size.x;
        int terrainHeight = (int) td.size.y;

        float x = ((int) positionToGenerateFor.x / terrainWidth) * terrainWidth;
        if (positionToGenerateFor.x < 0)
        {
            x -= terrainWidth;
        }

        float z = ((int) positionToGenerateFor.z / terrainHeight) * terrainHeight;
        if (positionToGenerateFor.z < 0)
        {
            z -= terrainHeight;
        }

        loadedTerrain.transform.position = new Vector3(x, 0, z);
        ChunkInfoStack.Push(new TerrainChunkInfo(loadedTerrain.transform.position, terrainWidth, terrainHeight));

        int heightmapWidth = td.heightmapWidth;
        int heightmapHeight = td.heightmapHeight;
        int max = (int) td.size.y;

        AsyncTerrainGenerator generator = new AsyncTerrainGenerator(HeightmapGenerator, TextureAlphaGenerator);
        generator.LoadNextChunkDataAsync(heightmapWidth, heightmapHeight, max, td.alphamapWidth, td.alphamapHeight);
        // yield until our chunk data is loaded.
        while (generator.AtomicIsLoadingNextChunkData)
        {
            yield return null;
        }

        // Set our heightmap and then yield (this takes a bit to be set.)
        td.SetHeights(0, 0, generator.NextHeightmap);
        yield return null;

        // Set our alphamap and then yield (this takes a bit to be set.)
        td.SetAlphamaps(0, 0, generator.NextAlphaMap);
        yield return null;
    }

    private class TerrainChunkInfo
    {
        public Vector3 Position;
        public int Width;
        public int Height;

        public TerrainChunkInfo(Vector3 position, int width, int height)
        {
            this.Position = position;
            this.Width = width;
            this.Height = height;
        }
    }
}
