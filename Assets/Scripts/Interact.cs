using UnityEngine;

public abstract class Interact : MonoBehaviour
{
    [SerializeField] protected bool canInteract;
    public abstract void InteractWithObject();

    public bool getCanInteract()
    {
        return canInteract; // Return the current state of canInteract
    }

}
