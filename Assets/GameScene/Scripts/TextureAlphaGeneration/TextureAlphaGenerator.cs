using UnityEngine;
using System.Collections;

public abstract class TextureAlphaGenerator : MonoBehaviour
{
    public abstract float[,,] GenerateTextureAlphas(float[,] heightmap, float maxHeight, int alphamapWidth, int alphamapHeight);
}
