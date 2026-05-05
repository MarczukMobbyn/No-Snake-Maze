using StarterAssets;
using UnityEngine;

public class SnakeJumpscare : MonoBehaviour
{
    [SerializeField] private GameObject snakeJumpscarePrefab; // Prefab for the snake jumpscare
    [SerializeField] Transform spawnPosition; // Position where the jumpscare will spawn
    StarterAssetsInputs playerInputs; // Reference to player inputs for interaction

    private GameObject snakeInstance; // Instance of the snake jumpscare

    public void Jumpscare()
    {
        if (snakeJumpscarePrefab != null && spawnPosition != null)
        {
            snakeInstance = FindFirstObjectByType<SnakeAI>().gameObject; // Find the first instance of SnakeAI in the scene
            if (snakeInstance != null)
            {
                Destroy(snakeInstance); // Destroy the previous instance if it exists
            }
            // Instantiate the snake jumpscare at the specified position
            Instantiate(snakeJumpscarePrefab, spawnPosition.position, spawnPosition.rotation);
            //turn off the player controls
            playerInputs = GetComponent<StarterAssetsInputs>();
            if (playerInputs != null)
            {
                playerInputs.playerMovementEnabled = false; // Disable player movement
                playerInputs.playerLookEnabled = false; // Disable player look
                playerInputs.move = Vector2.zero; // Reset movement input
                playerInputs.look = Vector2.zero; // Reset look input
            }
            else
            {
                Debug.LogWarning("StarterAssetsInputs component not found on the player.");
            }
            //play jumpscare sound here if needed
            Debug.Log("Snake jumpscare triggered!");
        }
        else
        {
            Debug.LogWarning("Snake jumpscare prefab or spawn position is not set.");
        }
    }

}
