using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Audio;
using UnityEngine.Audio;

public class OptionsScreen : MonoBehaviour
{
    
    public Toggle fullscreenToggle;
    public Toggle vsyncToggle;
    public TMP_Text resolutionText;

    public AudioMixer theMixer;

    public TMP_Text masterLabel;
    public TMP_Text musicLabel;
    public TMP_Text sfxLabel;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private int selectedResIndex = 0;


    public List<ResItem> resolutions = new List<ResItem>
    {
        new ResItem {horizontal = 2560, vertical = 1600 },
        new ResItem { horizontal = 2560, vertical = 1440 },
        new ResItem { horizontal = 1920, vertical = 1080 },
        new ResItem { horizontal = 1280, vertical = 720 },
        new ResItem { horizontal = 800, vertical = 600 }
    };

    private void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;
        if (QualitySettings.vSyncCount > 0)
        {
            vsyncToggle.isOn = true;
        }
        else
        {
            vsyncToggle.isOn = false;
        }

        bool foundRes = false;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                selectedResIndex = i;
                foundRes = true;
                UpdateResText();
                break;
            }
        }

        if (!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;
            newRes.vertical = Screen.height;

            resolutions.Add(newRes);
            selectedResIndex = resolutions.Count - 1;
            UpdateResText();
        }

        float vol = 0f;
        theMixer.GetFloat("MasterVol", out vol);
        masterSlider.value = vol;

        theMixer.GetFloat("SFXVol", out vol);
        sfxSlider.value = vol;

        theMixer.GetFloat("MusicVol", out vol);
        musicSlider.value = vol;

        masterLabel.text = Mathf.RoundToInt(masterSlider.value + 80).ToString();
        musicLabel.text = Mathf.RoundToInt(musicSlider.value + 80).ToString();
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value + 80).ToString();
    }

    public void ApplyGraphics()
    {
        //Screen.fullScreen = fullscreenToggle.isOn;
        if (vsyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1; // Enable VSync
        }
        else
        {
            QualitySettings.vSyncCount = 0; // Disable VSync
        }

        Screen.SetResolution(resolutions[selectedResIndex].horizontal, resolutions[selectedResIndex].vertical, fullscreenToggle.isOn);
    }

    public void ResLeft()
    {
        selectedResIndex--;
        if (selectedResIndex < 0)
        {
            selectedResIndex = resolutions.Count - 1; // Wrap around to the last resolution
        }

        UpdateResText();
    }

    public void ResRight()
    {
        selectedResIndex++;
        if (selectedResIndex >= resolutions.Count)
        {
            selectedResIndex = 0; // Wrap around to the first resolution
        }

        UpdateResText();
    }

    public void UpdateResText()
    {
        resolutionText.text = resolutions[selectedResIndex].horizontal.ToString() + " X " + resolutions[selectedResIndex].vertical.ToString();
    }

    public void SetMasterVolume()
    {
        masterLabel.text = Mathf.RoundToInt(masterSlider.value + 80).ToString();
        theMixer.SetFloat("MasterVol", masterSlider.value);

        PlayerPrefs.SetFloat("MasterVol", masterSlider.value);
    }
    public void SetMusicVolume()
    {
        musicLabel.text = Mathf.RoundToInt(musicSlider.value + 80).ToString();
        theMixer.SetFloat("MusicVol", musicSlider.value);

        PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
    }
    public void SetSFXVolume()
    {
        sfxLabel.text = Mathf.RoundToInt(sfxSlider.value + 80).ToString();
        theMixer.SetFloat("SFXVol", sfxSlider.value);

        PlayerPrefs.SetFloat("SFXVol", sfxSlider.value);
    }
}

[System.Serializable]
public class ResItem
{
    public int horizontal;
    public int vertical;
}
