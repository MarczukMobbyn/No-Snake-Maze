using System.Collections;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int damageAmount = 1; // Amount of damage the enemy deals
    [SerializeField] private float attackRange = 1.5f; // Range within which the enemy can attack
    [SerializeField] private float attackCooldown = 1f; // Time between attacks
    PlayerHealth playerHealth; // Reference to the PlayerHealth script

    [SerializeField] AudioSource attackSound; // Optional: Sound to play when attacking
    bool canAttack = true; // Flag to check if the enemy can attack

    SnakeAI snakeAI; // Reference to the SnakeAI script, if needed

    private void Start()
    {
        // Find the PlayerHealth component in the scene
        playerHealth = FindFirstObjectByType<PlayerHealth>().GetComponent<PlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth component not found on Player.");
        }

        snakeAI = GetComponent<SnakeAI>();
    }

    private void Update()
    {
        CheckDistance();
    }

    private void CheckDistance()
    {
        // Check if the player is within a certain distance to attack
        float distanceToPlayer = Vector3.Distance(transform.position, playerHealth.transform.position);
        Debug.DrawRay(transform.position, (playerHealth.transform.position - transform.position).normalized * distanceToPlayer, Color.red);
        if (distanceToPlayer < attackRange && canAttack) // Adjust the distance as needed
        {
            //check if enemy sees the player

            if (snakeAI.CanSeePlayer())
            {
                Attack();
            }
            
        }
    }

    void Attack()
    {
        if (playerHealth != null)
        {
            canAttack = false; // Set the flag to false to prevent immediate re-attack
            playerHealth.TakeDamage(damageAmount); // Call the TakeDamage method on PlayerHealth
            attackSound?.Play(); // Play attack sound if assigned
            Debug.Log("Enemy attacked the player for " + damageAmount + " damage.");
            // Start cooldown coroutine
            StartCoroutine(AttackCooldown());
        }
        else
        {
            Debug.LogWarning("PlayerHealth reference is null, cannot attack.");
        }
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown); // Wait for the cooldown period
        canAttack = true; // Reset the flag to allow attacking again
    }

}
