using UnityEngine;
using System.Collections;

public class TrackFlyCam : MonoBehaviour
{
    public float BaseMovementSpeed;
    public float HeightAboveTerrain;
    public ActiveTerrainSetter ActiveTerrainSetter;

    void Start ()
    {
	
	}
	
	void Update ()
    {
        float xForward = transform.forward.x * (BaseMovementSpeed) * Time.deltaTime;
        transform.position += new Vector3(xForward, 0, 0);

        float height = ActiveTerrainSetter.ActiveTerrain.SampleHeight(transform.position);
        float y = Mathf.Lerp(transform.position.y, height + HeightAboveTerrain, (float) 0.08);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
