using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [Header("Panel Root")]
    [SerializeField] private GameObject panelRoot;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI leaderboardText;

    private void Awake()
    {
        if (panelRoot == null)
            panelRoot = gameObject;

        panelRoot.SetActive(false);
    }

    
    public void Show(bool didWin, List<LeaderboardEntry> leaderboard)
    {
        // Oyunu durdur
        Time.timeScale = 0f;

        panelRoot.SetActive(true);

        
        if (resultText != null)
            resultText.text = didWin ? "YOU WIN!" : "YOU LOSE!";

        // Leaderboard
        if (leaderboardText != null)
        {
            if (leaderboard == null || leaderboard.Count == 0)
            {
                leaderboardText.text = "No games played yet.";
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("Leaderboard:");

                for (int i = 0; i < leaderboard.Count; i++)
                {
                    var e = leaderboard[i];
                    sb.AppendLine($"{i + 1}. {e.username}  W:{e.wins}  L:{e.losses}  G:{e.totalGames}");
                }

                leaderboardText.text = sb.ToString();
            }
        }
    }

    
    
    public void Hide()
    {
        panelRoot.SetActive(false);
        Time.timeScale = 1f;
    }

   
    /// Restart Button
    
    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

   
    /// Quit Button
   
    public void OnQuitButton()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
