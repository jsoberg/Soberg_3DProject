using UnityEngine;
using System.Collections;

public class ActiveTerrainSetter : MonoBehaviour
{
    public Terrain TerrainOne;
    public Terrain TerrainTwo;

    public Terrain ActiveTerrain { get; internal set; }

    public ActiveTerrainSetter()
    {
        ActiveTerrain = TerrainOne;
    }

    public float MaxDistance;

    void Update ()
    {
        Ray ray = new Ray(Camera.main.transform.position, new Vector3(0, -MaxDistance, 0));
        RaycastHit hit;
        if (TerrainOne.GetComponent<Collider>().Raycast(ray, out hit, MaxDistance)) {
            ActiveTerrain = TerrainOne;
        } else {
            ActiveTerrain = TerrainTwo;
        }
	}
}
