using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float drainRate = 30f; // Stamina drain rate
    public float regenRate = 15f;
    public float regenDelay = 1f;

    [Header("UI References")]
    public CanvasGroup canvasGroup;
    public Image staminaBar;

    [Header("Fade Settings")]
    public float fadeOutSpeed = 2f; // Im wy¿sza, tym szybciej znika

    private float currentStamina;
    private float regenTimer = 0f;
    private bool isSprinting = false;

    bool isChased = false; // Placeholder for chase state, can be set by other scripts


    void Start()
    {
        currentStamina = maxStamina;
        canvasGroup.alpha = 0f;
        UpdateUI();
    }

    void Update()
    {
        if (isSprinting)
        {
            DrainStamina();
        }
        else
        {

            //if stamina is drained to max, there's a delay before regen, but if it's not drained to max, stamina regens instantely
            regenTimer += Time.deltaTime;
            if (currentStamina <= 0f)
            {
                if (regenTimer >= regenDelay)
                {
                    RegenStamina();
                }
            }
            else
            {
                RegenStamina();
            }
        }

        UpdateUI();
    }



    public void SetSprinting(bool sprinting)
    {
        isSprinting = sprinting;
        if (sprinting)
            regenTimer = 0f;
    }

    public bool CanSprint()
    {
        return currentStamina > 0f;
    }

    void DrainStamina()
    {
        if (currentStamina > 0f)
        {
            currentStamina -= drainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    void RegenStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }
    }

    void UpdateUI()
    {
        float fill = currentStamina / maxStamina;
        staminaBar.fillAmount = fill;

        // UI fade logic
        if (fill < 1f || isSprinting)
        {
            // Pokazuje siê natychmiast
            canvasGroup.alpha = 1f;
        }
        else
        {
            // Zanika p³ynnie
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeOutSpeed * Time.deltaTime);
        }
    }

    public void AddStamina(float amount)
    {
        currentStamina += amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        UpdateUI();
    }
}
