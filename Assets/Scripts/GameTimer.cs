using System;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] float timePassed;

    [SerializeField] TMP_Text timerText; // Reference to the UI Text component to display the timer

    private void Start()
    {
        timePassed = 0f; // Initialize the timer to zero at the start
        if (timerText != null)
        {
            float time = PlayerPrefs.GetFloat("TimePassed", 0f); // load time in seconds
            UpdateTimerUI(time); // show in UI
        }
    }
    private void Update()
    {
        timePassed += Time.deltaTime; // Increment the timer by the time passed since the last frame
        //Debug.Log("Time Passed: " + timePassed); // Log the time passed for debugging purposes
    }

    public void SaveTimeToPlayerPrefs()
    {
        PlayerPrefs.SetFloat("TimePassed", timePassed); // Save the time passed to PlayerPrefs
        PlayerPrefs.Save(); // Ensure the data is saved immediately
    }

    private void UpdateTimerUI(float time)
    {
        if (timerText != null)
        {
            timerText.text = GetFormattedTime(time);
        }
    }

    public string GetFormattedTime(float time)
    {
        TimeSpan ts = TimeSpan.FromSeconds(time);
        // Jeœli chcesz pokazaæ godziny tylko jak >0, mo¿esz warunkowo formatowaæ:
        return string.Format("{0:D2}:{1:D2}:{2:D2}", ts.Hours, ts.Minutes, ts.Seconds);
        
    }
}
