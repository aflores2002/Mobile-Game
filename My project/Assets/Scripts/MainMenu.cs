using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI References")]
    public Button startButton;
    public CanvasGroup menuCanvas;

    [Header("Game Objects")]
    public GameObject gameplayUI;        // Combat UI elements
    public GameObject joystickCanvas;    // Joystick controls
    public GameObject playerObject;
    public EnemySpawner enemySpawner;

    void Start()
    {
        // Ensure references
        if (startButton == null)
        {
            Debug.LogError("MainMenu: Start button reference is missing!");
            return;
        }

        if (menuCanvas == null)
        {
            menuCanvas = GetComponent<CanvasGroup>();
        }

        // Initially disable all gameplay elements
        if (gameplayUI != null)
        {
            gameplayUI.SetActive(false);
        }

        if (joystickCanvas != null)
        {
            joystickCanvas.SetActive(false);
        }

        if (playerObject != null)
        {
            playerObject.SetActive(false);
        }

        if (enemySpawner != null)
        {
            enemySpawner.gameObject.SetActive(false);
        }

        // Add click listener
        startButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        Debug.Log("MainMenu: Starting game...");

        // Fade out menu
        LeanTween.alphaCanvas(menuCanvas, 0f, 0.5f).setOnComplete(() => {
            // Disable menu
            gameObject.SetActive(false);

            // Enable gameplay elements in the correct order
            if (playerObject != null)
            {
                playerObject.SetActive(true);
            }

            // Enable both UI canvases
            if (gameplayUI != null)
            {
                gameplayUI.SetActive(true);
            }

            if (joystickCanvas != null)
            {
                joystickCanvas.SetActive(true);
            }

            if (enemySpawner != null)
            {
                enemySpawner.gameObject.SetActive(true);
            }
        });
    }
}