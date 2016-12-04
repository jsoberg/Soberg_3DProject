using UnityEngine;
using System.Collections;

public class TrackFlyCam : MonoBehaviour
{
    public float BaseMovementSpeed;
    public float HeightAboveTerrain;
    public ActiveTerrainSetter ActiveTerrainSetter;

    public float DistanceForHeightCheck;
    public float LookForwardDistance;

    void Start ()
    {
    }

    private float pitch;

	void Update ()
    {
        float xForward = (BaseMovementSpeed) * Time.deltaTime;
        transform.position += new Vector3(xForward, 0, 0);

        if (ActiveTerrainSetter.ActiveTerrain == null) {
            return;
        }

        float directHeightUnder = ActiveTerrainSetter.ActiveTerrain.SampleHeight(transform.position);
        float heightCheckUnder = ActiveTerrainSetter.ActiveTerrain.SampleHeight(transform.position + new Vector3(DistanceForHeightCheck, 0, 0));

        float y = Mathf.Lerp(transform.position.y, Mathf.Max(directHeightUnder, heightCheckUnder) + HeightAboveTerrain, (float) 0.08);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        //LookTowardTerrainInDistance();
    }

    private void LookTowardTerrainInDistance()
    {
        Vector3 lookForward = transform.position + new Vector3(LookForwardDistance, 0, 0);
        Terrain terrainToLook = ActiveTerrainSetter.GetTerrainUnderWorldPoint(lookForward);
        float y = Mathf.Lerp(transform.position.y, terrainToLook.SampleHeight(lookForward), .12f);

        transform.LookAt(new Vector3(lookForward.x, y, lookForward.z));
        transform.Rotate(new Vector3(10,0,0));
    }
}
