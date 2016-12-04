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
        ActiveTerrain = GetTerrainUnderWorldPoint(Camera.main.transform.position);
	}

    public Terrain GetTerrainUnderWorldPoint(Vector3 worldPoint)
    {
        Ray downRay = new Ray(worldPoint, new Vector3(0, -MaxDistance, 0));
        Ray upRay = new Ray(worldPoint, new Vector3(0, MaxDistance, 0));

        Collider terrainCollider = TerrainOne.GetComponent<Collider>();
        RaycastHit hit;
        if (terrainCollider.Raycast(downRay, out hit, MaxDistance) || terrainCollider.Raycast(upRay, out hit, MaxDistance)) {
            return TerrainOne;
        } else {
            return TerrainTwo;
        }
    }
}
