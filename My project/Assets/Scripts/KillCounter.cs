using UnityEngine;
using TMPro;

public class KillCounter : MonoBehaviour
{
    public TextMeshProUGUI killCountText;
    private int killCount = 0;

    void Start()
    {
        // Initialize text
        UpdateKillText();

        // Find all BaseEnemy instances and subscribe to their death events
        BaseEnemy[] enemies = FindObjectsOfType<BaseEnemy>();
        foreach (BaseEnemy enemy in enemies)
        {
            enemy.onDeath.AddListener(IncrementKills);
        }
    }

    void OnEnable()
    {
        // Reset kill count when enabled
        killCount = 0;
        UpdateKillText();
    }

    public void IncrementKills()
    {
        killCount++;
        UpdateKillText();
    }

    private void UpdateKillText()
    {
        if (killCountText != null)
        {
            killCountText.text = $"Kills: {killCount}";
        }
    }

    // Method to subscribe to new enemies that spawn during gameplay
    public void SubscribeToEnemy(BaseEnemy enemy)
    {
        if (enemy != null)
        {
            enemy.onDeath.AddListener(IncrementKills);
        }
    }
}