using StarterAssets;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject interactUI; // Reference to the UI element for interaction
    public GameObject pauseOptions;
    public GameObject optionsPanel; // Reference to the options panel
    public GameObject howToPlayPanel; // Reference to the how-to-play panel
    public StarterAssetsInputs starterAssetsInputs; // Reference to the StarterAssetsInputs script

    public SceneManagerObject sceneManagerObject; // Reference to the SceneManagerObject script

    private float loadMenuTime = 1f;

    bool isPaused = false;

    public void ShowInteractUI(bool state)
    {
        if (interactUI == null)
        {
            Debug.LogError("Interact UI is not assigned in the UIManager.");
            return;
        }
        interactUI.SetActive(state); // Show or hide the interaction UI based on the state
    }

    private void Update()
    {
        if (starterAssetsInputs == null)
        {
            Debug.LogError("StarterAssetsInputs is not assigned in the UIManager.");
            return;
        }

        // Example: Toggle the interact UI with the 'E' key
        if (Input.GetKeyDown(KeyCode.Escape) && !optionsPanel.activeSelf)
        {
            pauseOptions.SetActive(!pauseOptions.activeSelf);
            starterAssetsInputs.playerMovementEnabled = !pauseOptions.activeSelf;
            starterAssetsInputs.playerLookEnabled = !pauseOptions.activeSelf;
            starterAssetsInputs.SetCursorState(!pauseOptions.activeSelf);
            starterAssetsInputs.move = Vector2.zero; // Reset movement input when paused
            starterAssetsInputs.look = Vector2.zero; // Reset look input when paused
        }
    }

    public void ShowOptions()
    {
        if (optionsPanel == null)
        {
            Debug.LogError("Options Panel is not assigned in the UIManager.");
            return;
        }
        optionsPanel.SetActive(true); // Show the options panel
        pauseOptions.SetActive(false); // Hide the pause options

    }    

    public void CloseOptions()
    {
        if (optionsPanel == null)
        {
            Debug.LogError("Options Panel is not assigned in the UIManager.");
            return;
        }
        optionsPanel.SetActive(false); // Hide the options panel
        pauseOptions.SetActive(true); // Show the pause options
    }

    public void ResumeGame()
    {
        if (pauseOptions == null)
        {
            Debug.LogError("Pause Options is not assigned in the UIManager.");
            return;
        }
        pauseOptions.SetActive(false); // Hide the pause options
        starterAssetsInputs.playerMovementEnabled = true; // Enable player movement
        starterAssetsInputs.playerLookEnabled = true; // Enable player look
        starterAssetsInputs.SetCursorState(true); // Lock the cursor
        starterAssetsInputs.move = Vector2.zero; // Reset movement input when resuming
        starterAssetsInputs.look = Vector2.zero; // Reset look input when resuming
    }
    
    public void BackToMainMenu()
    {
        if (sceneManagerObject == null)
        {
            return;
        }
        sceneManagerObject.LoadMainMenu(loadMenuTime); // Call the method to load the main menu
    }

    public void OpenHowToPlay()
    {
        if (howToPlayPanel == null)
        {
            Debug.LogError("Options Panel is not assigned in the UIManager.");
            return;
        }
        howToPlayPanel.SetActive(true); // Show the options panel
        pauseOptions.SetActive(false); // Hide the pause options
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel == null)
        {
            Debug.LogError("Options Panel is not assigned in the UIManager.");
            return;
        }
        howToPlayPanel.SetActive(false); // Hide the options panel
        pauseOptions.SetActive(true); // Show the pause options
    }

}

