using UnityEngine;
using UnityEngine.InputSystem;

public class ControlCursorGameOver : MonoBehaviour
{
    private void Start()
    {
        SetCursorState(false); // Unlock the cursor at the start
    }

    public void SetCursorState(bool isLocked)
    {
        if (isLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
