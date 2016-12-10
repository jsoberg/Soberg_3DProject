using UnityEngine;
using System.Collections;

public class ActiveTerrainSetter : MonoBehaviour
{
    public Terrain ActiveTerrain { get; internal set; }

    public ActiveTerrainSetter()
    {
        ActiveTerrain = null;
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

        //Collider myCollider = GetComponent<Collider>();
        //RaycastHit hit;
        //if (myCollider.Raycast(downRay, out hit, MaxDistance)) {
         //   return null; //(Terrain) hit.collider.gameObject;
        //} else if (myCollider.Raycast(upRay, out hit, MaxDistance)) {
        //    return null; //(Terrain) hit.collider.gameObject;
        //}
        return null;
    }
}
