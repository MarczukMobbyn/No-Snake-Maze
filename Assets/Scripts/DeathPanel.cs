using UnityEngine;

public class DeathPanel : MonoBehaviour
{

    [SerializeField] CanvasGroup gameOverPanel; // Reference to the game over panel
    [SerializeField] float fadeSpeed = 0.5f; // Speed at which the game over panel fades in
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameOverPanel.alpha = 0f; // Hide game over panel at start
    }

    private void Update()
    {
        gameOverPanel.alpha = Mathf.Lerp(gameOverPanel.alpha, 1f, fadeSpeed * Time.deltaTime); // Fade in the game over panel
    }


}
