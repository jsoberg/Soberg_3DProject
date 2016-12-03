using UnityEngine;
using System.Collections;

public class TrackFlyCam : MonoBehaviour
{
    public float BaseMovementSpeed;
    public float HeightAboveTerrain;
    public ActiveTerrainSetter ActiveTerrainSetter;

    public float DistanceForHeightCheck;

    void Start ()
    {
	
	}
	
	void Update ()
    {
        float xForward = transform.forward.x * (BaseMovementSpeed) * Time.deltaTime;
        transform.position += new Vector3(xForward, 0, 0);

        if (ActiveTerrainSetter.ActiveTerrain == null) {
            return;
        }

        float directHeightUnder = ActiveTerrainSetter.ActiveTerrain.SampleHeight(transform.position);
        float heightCheckUnder = ActiveTerrainSetter.ActiveTerrain.SampleHeight(transform.position + new Vector3(DistanceForHeightCheck, 0, 0));

        float y = Mathf.Lerp(transform.position.y, Mathf.Max(directHeightUnder, heightCheckUnder) + HeightAboveTerrain, (float) 0.08);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
