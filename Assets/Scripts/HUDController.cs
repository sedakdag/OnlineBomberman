using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerPowerStats playerStats;
    [SerializeField] private PlayerHealth playerHealth; // can sistemi buradan gelecek

    [Header("Hearts")]
    [SerializeField] private Image[] hearts; // Heart_1..3

    [Header("Texts")]
    [SerializeField] private TMP_Text bombText;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private TMP_Text speedText;

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject topLeftPanel;

    private void Start()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(false);

        RefreshStats();
        RefreshHearts();
    }

    private void OnEnable()
    {
        if (playerStats != null)
            playerStats.Changed += RefreshStats;
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.Changed -= RefreshStats;
    }
    
    public void RefreshStats()
    {
        if (playerStats == null) return;

        if (bombText)  bombText.text  = $"Bombs: {playerStats.bombCount}";
        if (powerText) powerText.text = $"Power: {playerStats.bombPower}";
        if (speedText) speedText.text = $"Speed: {playerStats.speed:0.0}";
    }

    public void RefreshHearts()
    {
        if (playerHealth == null || hearts == null) return;

        int hp = playerHealth.CurrentHP;
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null)
                hearts[i].enabled = (i < hp);
        }
    }

    public void ShowGameOver()
    {
        if (topLeftPanel) topLeftPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }


    public void ShowWin()
    {
        if (winPanel) winPanel.SetActive(true);
    }
}
