using StarterAssets;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField] Transform player;

    private void Awake()
    {
        player = FindAnyObjectByType<FirstPersonController>().GetComponent<Transform>();
    }

    private void Update()
    {
        if (player != null)
        {
            // Update the position of this GameObject to follow the player
            transform.position = new Vector3(player.position.x, transform.position.y, player.position.z);

            // Optionally, make this GameObject look at the player
            //transform.LookAt(player);
        }
        else
        {
            Debug.LogWarning("Player Transform is not assigned in PlayerFollow script.");
        }
    }
}
