using UnityEngine;
using System.Collections;

public class RingGenerator : MonoBehaviour
{
    private const float ValueAheadOfMainCharacterToPlaceRing = 10000;

    public GameObject MainCharacter;
    public GameObject RingPrefab;

    public float BaseSecondsToGenerateNewRing;

    private float TimeSinceLastGenerate;

	void Update ()
    {
        TimeSinceLastGenerate += Time.deltaTime;
        if (ShouldGenerateRing()) {
            GenerateRingForMainCharacter();
            TimeSinceLastGenerate = 0f;
        }
	}

    private bool ShouldGenerateRing()
    {
        return (TimeSinceLastGenerate > BaseSecondsToGenerateNewRing);
    }

    private void GenerateRingForMainCharacter()
    {
        Vector3 currentPosition = MainCharacter.transform.position;
        // TODO put ring somewhere to the left/right of character so they have to move to it.
        currentPosition.x += ValueAheadOfMainCharacterToPlaceRing;

        Instantiate(RingPrefab, currentPosition, Quaternion.identity);
    }
}
