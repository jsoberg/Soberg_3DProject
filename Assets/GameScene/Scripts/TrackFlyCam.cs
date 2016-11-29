using UnityEngine;
using System.Collections;

public class TrackFlyCam : MonoBehaviour
{
    public float BaseMovementSpeed;

    void Start ()
    {
	
	}
	
	void Update ()
    {
        transform.position += transform.forward * (BaseMovementSpeed) * Time.deltaTime;
    }
}
