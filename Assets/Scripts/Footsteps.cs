using UnityEngine;
using StarterAssets;

public class Footsteps : MonoBehaviour
{
    StarterAssetsInputs starterAssetsInputs;

    [SerializeField] AudioSource walkAudio;
    [SerializeField] AudioSource sprintAudio;

    private void Start()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();

        walkAudio.Stop();
        sprintAudio.Stop();
    }

    private void Update()
    {
        bool isMoving = starterAssetsInputs.move != Vector2.zero;
        bool isSprinting = starterAssetsInputs.sprint;

        if (isMoving)
        {
            if (isSprinting)
            {
                if (!sprintAudio.isPlaying)
                {
                    walkAudio.Stop();
                    sprintAudio.Play();
                }
            }
            else
            {
                if (!walkAudio.isPlaying)
                {
                    sprintAudio.Stop();
                    walkAudio.Play();
                }
            }
        }
        else
        {
            if (walkAudio.isPlaying) walkAudio.Stop();
            if (sprintAudio.isPlaying) sprintAudio.Stop();
        }
    }
}
