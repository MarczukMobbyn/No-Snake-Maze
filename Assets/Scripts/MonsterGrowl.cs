using System.Collections;
using UnityEngine;

public class MonsterGrowl : MonoBehaviour
{
    [SerializeField] AudioSource growlSound; // Reference to the AudioSource for the growl sound
    [SerializeField] float minInterval = 40f;
    [SerializeField] float maxInterval = 60f;
    void Start()
    {
        growlSound?.Play(); // Play the growl sound at the start if assigned
        StartCoroutine(GrowlRoutine()); // Start the growl routine
    }

    IEnumerator GrowlRoutine()
    {
        while (true)
        {
            // Wait for a random interval between minInterval and maxInterval
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);
            // Play the growl sound
            growlSound?.Play();
        }
    }


}
