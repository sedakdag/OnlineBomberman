using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerPowerStats playerStats;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Hearts")]
    [SerializeField] private Image[] hearts; // Heart_1..3

    [Header("Top Left Texts")]
    [SerializeField] private TMP_Text bombText;
    [SerializeField] private TMP_Text powerText;
    [SerializeField] private TMP_Text speedText;

    [Header("Result UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject topLeftPanel;   // Bombs/Power/Speed paneli
    [SerializeField] private TMP_Text resultTitleText;  // büyük başlık (GAME OVER / YOU WIN)
    [SerializeField] private TMP_Text leaderboardText;  // leaderboard yazısı

    private void Start()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (winPanel)      winPanel.SetActive(false);

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

    // === Canlar / Bomb / Power / Speed ===

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

    // === Sonuç ekranı gösterme (parametreli & paramsız) ===

    // Eski çağrıları bozmasın diye parametresiz versiyonları bıraktık
    public void ShowGameOver()
    {
        ShowGameOver(null);
    }

    public void ShowWin()
    {
        ShowWin(null);
    }

    // Server’dan gelecek leaderboard string’i buraya verebiliriz
    public void ShowGameOver(string leaderboardTextContent)
    {
        if (topLeftPanel) topLeftPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(true);
        if (winPanel) winPanel.SetActive(false);

        if (resultTitleText) resultTitleText.text = "GAME OVER";

        if (leaderboardText)
            leaderboardText.text = leaderboardTextContent ?? string.Empty;
    }

    public void ShowWin(string leaderboardTextContent)
    {
        if (topLeftPanel) topLeftPanel.SetActive(false);
        if (winPanel) winPanel.SetActive(true);
        if (gameOverPanel) gameOverPanel.SetActive(false);

        if (resultTitleText) resultTitleText.text = "YOU WIN!";

        if (leaderboardText)
            leaderboardText.text = leaderboardTextContent ?? string.Empty;
    }
}
