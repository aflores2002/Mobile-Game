using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PowerAttackUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI References")]
    public Image chargeBar;
    public AttackButton normalAttackButton;
    public Image attackButtonImage; // Reference to the attack button's image

    [Header("Charge Settings")]
    public float timeToFullCharge = 1.0f;
    public float minChargeTimeForPowerAttack = 0.3f;

    [Header("Visual Settings")]
    public Color emptyColor = Color.gray;
    public Color fullColor = Color.red;

    private bool isCharging = false;
    public bool IsCharging => isCharging;
    private float currentCharge = 0f;
    private CatKnightController playerController;
    private AttackManager attackManager;
    private Coroutine chargeCoroutine;
    private float pressStartTime;

    void Start()
    {
        // Find references
        playerController = FindObjectOfType<CatKnightController>();
        attackManager = playerController.GetComponent<AttackManager>();

        // Initialize charge bar
        chargeBar.fillAmount = 0f;
        chargeBar.color = emptyColor;

        // Ensure the charge bar starts invisible
        chargeBar.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pressStartTime = Time.time;
        // Only start charging, don't trigger attack
        isCharging = true;
        chargeBar.gameObject.SetActive(true);
        chargeCoroutine = StartCoroutine(ChargeRoutine());

        // Add visual feedback for button press
        if (attackButtonImage != null)
        {
            attackButtonImage.color = new Color(
                attackButtonImage.color.r * 0.8f,
                attackButtonImage.color.g * 0.8f,
                attackButtonImage.color.b * 0.8f,
                attackButtonImage.color.a
            );
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        float pressDuration = Time.time - pressStartTime;

        if (isCharging)
        {
            isCharging = false;
            StopCoroutine(chargeCoroutine);

            // Execute appropriate attack based on charge duration
            if (currentCharge >= minChargeTimeForPowerAttack)
            {
                ExecutePowerAttack();
            }
            else
            {
                ExecuteNormalAttack();
            }

            // Reset charge bar
            ResetChargeBar();
        }
    }

    private IEnumerator ChargeRoutine()
    {
        float startTime = Time.time;

        while (isCharging)
        {
            // Calculate charge progress
            float elapsedTime = Time.time - startTime;
            currentCharge = Mathf.Clamp01(elapsedTime / timeToFullCharge);

            // Update visual feedback
            chargeBar.fillAmount = currentCharge;
            chargeBar.color = Color.Lerp(emptyColor, fullColor, currentCharge);

            yield return null;
        }
    }

    private void ExecutePowerAttack()
    {
        Debug.Log("Executing Power Attack!");
        if (playerController != null)
        {
            playerController.OnPowerAttackInput();
        }
    }

    private void ExecuteNormalAttack()
    {
        Debug.Log("Executing Normal Attack!");
        if (playerController != null)
        {
            playerController.OnAttackInput();
        }
    }

    private void ResetChargeBar()
    {
        currentCharge = 0f;
        chargeBar.fillAmount = 0f;
        chargeBar.color = emptyColor;
        chargeBar.gameObject.SetActive(false);

        // Reset button color
        if (attackButtonImage != null)
        {
            attackButtonImage.color = Color.white; // Or whatever your normal color is
        }
    }
}