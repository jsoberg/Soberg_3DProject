using UnityEngine;
using System.Collections;

public abstract class HeightmapGenerator : MonoBehaviour
{
    public abstract float[,] GenerateHeightMap(int width, int height, int max);
}
