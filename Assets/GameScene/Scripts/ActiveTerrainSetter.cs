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
        Ray downRay = new Ray(Camera.main.transform.position, new Vector3(0, -MaxDistance, 0));
        Ray upRay = new Ray(Camera.main.transform.position, new Vector3(0, MaxDistance, 0));

        Collider terrainCollider = TerrainOne.GetComponent<Collider>();
        RaycastHit hit;
        if (terrainCollider.Raycast(downRay, out hit, MaxDistance) || terrainCollider.Raycast(upRay, out hit, MaxDistance)) {
            ActiveTerrain = TerrainOne;
        } else {
            ActiveTerrain = TerrainTwo;
        }
	}
}
