using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AttackButton : MonoBehaviour, IPointerDownHandler
{
    private CatKnightController playerController;
    private Image buttonImage;
    private Color normalColor;
    private Color pressedColor;
    private PowerAttackUI powerAttackUI;  // Reference to power attack component

    void Start()
    {
        Debug.Log("AttackButton: Initializing");

        // Find the player controller
        playerController = FindObjectOfType<CatKnightController>();
        if (playerController == null)
        {
            Debug.LogError("AttackButton: No CatKnightController found!");
            return;
        }

        // Find PowerAttackUI
        powerAttackUI = FindObjectOfType<PowerAttackUI>();
        if (powerAttackUI == null)
        {
            Debug.LogError("AttackButton: No PowerAttackUI found!");
        }

        // Get the button image component
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            normalColor = buttonImage.color;
            pressedColor = new Color(
                normalColor.r * 0.8f,
                normalColor.g * 0.8f,
                normalColor.b * 0.8f,
                normalColor.a
            );
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Don't trigger normal attack if power attack UI is charging
        if (powerAttackUI != null && powerAttackUI.IsCharging)
        {
            return;
        }

        Debug.Log("AttackButton: Button pressed");

        // Visual feedback
        if (buttonImage != null)
        {
            buttonImage.color = pressedColor;
        }

        // Trigger attack
        if (playerController != null)
        {
            playerController.OnAttackInput();
        }

        // Schedule color reset
        Invoke("ResetButtonColor", 0.1f);
    }

    private void ResetButtonColor()
    {
        if (buttonImage != null && !powerAttackUI.IsCharging)
        {
            buttonImage.color = normalColor;
        }
    }

    public void ForceButtonColor(Color color)
    {
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
    }
}