using StarterAssets;
using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3; // Maximum health of the player
    public int playerHealth;
    public CanvasGroup gameOverPanel;
    [SerializeField] float fadeSpeed = .5f; // Speed at which the game over panel fades in

    SnakeJumpscare snakeJumpscare; // Reference to the SnakeJumpscare script (if needed for jumpscare functionality)

    [SerializeField] GameObject[] bloodScreens; // Array of blood screen effects

    SceneManagerObject sceneManagerObject; // Reference to the SceneManagerObject script (if needed for scene management)

    StarterAssetsInputs inputs;
    StaminaController staminaController; // Reference to the StaminaController script (if needed for stamina management)

    [HideInInspector] public bool playerExhausted = false; // Flag to check if the player is exhausted
    [SerializeField] float playerExhaustedTime = 2f;

    bool isPlayerDead = false;

    private void Start()
    {
        playerHealth = maxHealth; // Initialize player health
        gameOverPanel.alpha = 0f; // Hide game over panel at start
        foreach (var bloodScreen in bloodScreens)
        {
            bloodScreen.SetActive(false); // Hide all blood screen effects at start
        }
        snakeJumpscare = GetComponent<SnakeJumpscare>(); // Get the SnakeJumpscare component if needed
        sceneManagerObject = FindFirstObjectByType<SceneManagerObject>(); // Find the SceneManagerObject in the scene
        staminaController = GetComponent<StaminaController>(); // Get the StaminaController component if needed
    }
    public void TakeDamage(int amount)
    {
        playerHealth -= amount;
        staminaController?.AddStamina(50f); // Add stamina when taking damage, if StaminaController exists, 50f is half of the stamina bar
        playerExhausted = true; // Set enemy attacked state to true
        StartCoroutine(ChangePlayerExhaustion(playerExhaustedTime)); // Start coroutine to reset player exhaustion after a delay

        if (playerHealth <= .7f *maxHealth)
        {
            bloodScreens[0].SetActive(true); // Show first blood screen effect
        }
        if (playerHealth <= .35f * maxHealth)
        {
            bloodScreens[1].SetActive(true); // Show second blood screen effect
        }


        if (playerHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator ChangePlayerExhaustion(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); // Wait for 1 second before changing enemyAttacked to false
        playerExhausted = false; // Reset enemy attacked state
    }

    void Die()
    {
        
        snakeJumpscare?.Jumpscare(); // Trigger the snake jumpscare if the component exists

        //after 2 seconds of jumpscare black screen and change scenes to "you died scene"
        Invoke("ShowBlackPanel", 2f); // Show game over panel after 2 seconds

        //Time.timeScale = 0f; // Pause the game
        Debug.Log("Player has died.");
    }

    void ShowBlackPanel()
    {
        isPlayerDead = true; // Set player dead state
        if (sceneManagerObject != null)
        {
            StartCoroutine(UnlockCursorRoutine(2f));
            sceneManagerObject.LoadNextScene(3f);

        }
        else
        {
            Debug.LogWarning("SceneManagerObject not found in the scene.");
        }

    }
    private void Update()
    {
        if (isPlayerDead)
        {
            gameOverPanel.gameObject.SetActive(true); // Ensure the game over panel is active
            gameOverPanel.alpha = Mathf.MoveTowards(gameOverPanel.alpha, 1f, fadeSpeed * Time.deltaTime); // Fade in the game over panel
        }
    }

    IEnumerator UnlockCursorRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        UnlockCursor();
    }
    void UnlockCursor()
    {
        inputs.SetCursorState(false);
    }

}
