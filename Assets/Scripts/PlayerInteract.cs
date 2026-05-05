using StarterAssets;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] string INTERACT_TAG = "Interactable";
    [SerializeField] StarterAssetsInputs inputs; // Assuming you have a reference to StarterAssetsInputs
    [SerializeField] UIManager uiManager; // Assuming you have a reference to UIManager
    RaycastHit hit;

    private void Start()
    {

        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("UIManager not found in the scene.");
            }
        }
    }
    private void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f))
        {
            if (hit.collider.CompareTag(INTERACT_TAG))
            {
                var interact = hit.collider.GetComponent<Interact>();
                if (interact.getCanInteract())
                {
                    uiManager.ShowInteractUI(true); // Show interaction UI
                }

                if (inputs.interact)
                {
                    interact.InteractWithObject();
                }
            }
            else
            {
                uiManager.ShowInteractUI(false); // Hide interaction UI
            }

        }
        else
        {
            uiManager.ShowInteractUI(false); // Hide interaction UI if nothing is hit
        }

    }
}
