using UnityEngine;

public class WinManager : MonoBehaviour
{
    [SerializeField] LeverInteract[] levers;
    [SerializeField] int leversPulledCount = 0;

    [SerializeField] HatchInteract hatchInteract; // Reference to the HatchInteract script to open the hatch

    // This script manages the win condition by tracking lever pulls
    private void Start()
    {
        levers = FindObjectsByType<LeverInteract>(FindObjectsSortMode.None);
        hatchInteract = FindFirstObjectByType<HatchInteract>();
    }

    public void LeverPulled()
    {
        leversPulledCount++;
        Debug.Log("Lever Pulled: " + leversPulledCount);
        if (leversPulledCount >= levers.Length)
        {
            //Opening Hatch
            hatchInteract.playAnimation();
        }
    }
}
