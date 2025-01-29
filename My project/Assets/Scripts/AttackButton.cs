using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AttackButton : MonoBehaviour, IPointerDownHandler
{
    private CatKnightController playerController;
    private Image buttonImage;
    private Color normalColor;
    private Color pressedColor;

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
        Debug.Log("AttackButton: Successfully found CatKnightController");

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
            Debug.Log("AttackButton: Image component initialized");
        }
    }

    void Awake()
    {
        Debug.Log("AttackButton: Awake called");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("AttackButton: Button pressed");
        Debug.Log($"AttackButton: Event position = {eventData.position}");

        // Visual feedback
        if (buttonImage != null)
        {
            buttonImage.color = pressedColor;
            Debug.Log("AttackButton: Changed button color");
        }

        // Trigger attack
        if (playerController != null)
        {
            Debug.Log("AttackButton: Calling OnAttackInput on player controller");
            playerController.OnAttackInput();
        }
        else
        {
            Debug.LogError("AttackButton: playerController is null when trying to attack!");
        }

        // Schedule color reset
        Invoke("ResetButtonColor", 0.1f);
    }

    private void ResetButtonColor()
    {
        if (buttonImage != null)
        {
            buttonImage.color = normalColor;
            Debug.Log("AttackButton: Reset button color");
        }
    }
}