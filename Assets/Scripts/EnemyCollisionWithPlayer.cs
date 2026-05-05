using UnityEngine;

public class EnemyCollisionWithPlayer : MonoBehaviour
{
    [SerializeField] SnakeAI head;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 playerPosition = collision.transform.position;
            head?.HearNoise(playerPosition); 
        }
    }
}
