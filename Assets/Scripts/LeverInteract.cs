using System.Collections;
using UnityEngine;

public class LeverInteract : Interact
{
    Animator leverAnimator;
    [SerializeField] Material leverMaterial; // Optional: Material to change when the lever is pulled
    [SerializeField] AnimationClip leverAnimationClip; // Optional: Animation clip for the lever pull
    [SerializeField] MeshRenderer leverMeshRenderer; // Optional: MeshRenderer to change the material
    WinManager winManager; // Reference to WinManager to track lever pulls
    SnakeSpawner snakeSpawner;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource beepSound;
    
    private void Start()
    {
        canInteract = true; // Initialize canInteract to true
        leverAnimator = GetComponent<Animator>();
        if (leverAnimator == null)
        {
            Debug.LogError("Animator component not found on LeverInteract object.");
        }
    }

    public void Init(WinManager winManager, SnakeSpawner snakeSpawner)
    {
        this.winManager = winManager; // Initialize the WinManager reference
        this.snakeSpawner = snakeSpawner; // Initialize the SnakeSpawner reference

    }
    public override void InteractWithObject()
    {
        if (!canInteract) return;
        canInteract = false;

        Debug.Log("Interacted With Lever");
        beepSound?.Stop(); // Stop the beep sound if it's playing
        audioSource?.Play(); // Play the lever pull sound if AudioSource is assigned
        leverAnimator.SetTrigger("PullLever");
        StartCoroutine(changeMaterial()); // Start the coroutine to change the material
        winManager.LeverPulled(); // Notify WinManager that the lever has been pulled
        snakeSpawner.SpawnSnake(); // Spawn the snake when the lever is pulled
        snakeSpawner.EmitNoise(transform.position); // Emit noise to alert the snake
        
    }

    IEnumerator changeMaterial()
    {
        if (leverMaterial != null)
        {
            // Change the material color to indicate interaction
            yield return new WaitForSeconds(leverAnimationClip.length); // Wait for half a second
            leverMeshRenderer.material = leverMaterial; // Change the material
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.5f, 0.3f);
        Gizmos.DrawIcon(transform.position, "sv_label_0", true); // ikona
    }
#endif
}


