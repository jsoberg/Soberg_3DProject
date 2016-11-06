using UnityEngine;
using System.Collections;
using System;

public class TestHeightmapGenerator : HeightmapGenerator
{
    public override float[,] GenerateHeightMap(int width, int height, int max)
    {
        float[,] heightmap = new float[width, height];

        int curr = max;
        for(int i = 0; i < width; i ++) {
            for (int j = 0; j < height; j ++) {
                heightmap[i, j] = curr;
            }
            curr-= 10;
        }
        return heightmap;
    }
}
