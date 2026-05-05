using System.Collections.Generic;
using UnityEngine;

public class HatchInteract : Interact
{

    private SnakeSpawner snakeSpawner;
    AudioSource audioSource;
    GameTimer gameTimer;
    SceneManagerObject sceneManagerObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canInteract = false; // Initialize canInteract to false
        audioSource = GetComponent<AudioSource>();
    }

    public void Init(SnakeSpawner snakeSpawner, GameTimer gameTimer, SceneManagerObject sceneManager)
    {
        this.snakeSpawner = snakeSpawner;
        this.gameTimer = gameTimer; // Store the reference to the SnakeSpawner and GameTimer
        this.sceneManagerObject = sceneManager; // Store the reference to the SceneManagerObject
    }

    public void setInteract(bool value)
    {
        canInteract = value; // Set the canInteract state
    }

    public override void InteractWithObject()
    {
        if (!canInteract) return;
        canInteract = false;

        // Winning game shit
        //saving players escape time to player prefs
        gameTimer.SaveTimeToPlayerPrefs();
        //change scene to "you escaped scene"
        sceneManagerObject.LoadVictoryScene(2f); // Load the victory scene after 2 seconds
        Debug.Log("CONGRATULATIONS, YOU ESCAPED!!!");

    }

    public void playAnimation()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("OpenHatch");
            snakeSpawner.EmitNoise(transform.position);
            setInteract(true); // Set canInteract to true after the animation is played
            audioSource?.Play(); // Play the hatch opening sound if AudioSource is assigned
        }
        else
        {
            Debug.LogError("Animator component not found on HatchInteract object.");
        }
    }


}
