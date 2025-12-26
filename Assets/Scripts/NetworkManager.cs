using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Contracts;   // BombData, MoveState, NetworkEvents

public class NetworkManager : MonoBehaviour
{
    // ---- Singleton ----
    public static NetworkManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---- SignalR ----
    public HubConnection connection;

    [Header("Remote Prefabs")]
    public GameObject remotePlayerPrefab;      // Kırmızı kare
    public GameObject remoteBombPrefab;        // Remote bomba
    public GameObject remoteExplosionPrefab;   // Remote patlama efekti

    [SerializeField] private HUDController hud;

    [Header("Game Over UI")]
    [SerializeField] private GameOverUI gameOverUI;

    [Header("Player")]
    [SerializeField] private string playerName = "Unity_Test_Oyuncusu";

    // Diğer oyuncular
    private readonly Dictionary<string, GameObject> otherPlayers =
        new Dictionary<string, GameObject>();

    // Remote bombalar (bombId -> GameObject)
    private readonly Dictionary<string, GameObject> remoteBombs =
        new Dictionary<string, GameObject>();

    // --- BAĞLANTI KURULUYOR ---
    private async void Start()
    {
        connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5100/gameHub")
            .WithAutomaticReconnect()
            .Build();

        // === Event handler'lar ===

        // 1) Hareket dinleme
        connection.On<MoveState>(NetworkEvents.PlayerMoved, (data) =>
        {
            Debug.Log($"[CLIENT] PlayerMoved event geldi: {data.playerName} -> ({data.x}, {data.y})");
            MainThreadDispatcher.Enqueue(() =>
            {
                OnRemotePlayerMoved(data);
            });
        });

        // 2) BombPlaced dinleme
        connection.On<BombData>(NetworkEvents.BombPlaced, (bomb) =>
        {
            Debug.Log($"[CLIENT] BombPlaced event geldi: {bomb.ownerId} -> ({bomb.x}, {bomb.y}) power={bomb.power}");
            MainThreadDispatcher.Enqueue(() =>
            {
                OnRemoteBombPlaced(bomb);
            });
        });

        // 3) BombExploded dinleme
        connection.On<BombData>(NetworkEvents.BombExploded, (bomb) =>
        {
            Debug.Log($"[CLIENT] BombExploded event geldi: ({bomb.x}, {bomb.y}) power={bomb.power}");
            MainThreadDispatcher.Enqueue(() =>
            {
                OnRemoteBombExploded(bomb);
            });
        });

        // 4) PlayerDied dinleme
        connection.On<string>(NetworkEvents.PlayerDied, (deadId) =>
        {
            MainThreadDispatcher.Enqueue(() =>
            {
                Debug.Log($"[CLIENT] PlayerDied event geldi: {deadId}");
                OnPlayerDied(deadId);
            });
        });

        // --- Otomatik reconnect logları ---
        connection.Reconnecting += error =>
        {
            Debug.LogWarning($"[NetworkManager] Reconnecting... error={error?.Message}");
            return Task.CompletedTask;
        };

        connection.Reconnected += id =>
        {
            Debug.Log($"[NetworkManager] Reconnected. New ConnectionId={id}");
            return Task.CompletedTask;
        };

        connection.Closed += error =>
        {
            Debug.LogWarning($"[NetworkManager] Closed. error={error?.Message}");
            return Task.CompletedTask;
        };

        await ConnectAndJoin();
    }

    private async Task ConnectAndJoin()
    {
        try
        {
            await connection.StartAsync();
            Debug.Log($"[NetworkManager] StartAsync bitti. State={connection.State}, ConnectionId='{connection.ConnectionId}'");

            if (connection.State == HubConnectionState.Connected)
            {
                await connection.InvokeAsync("JoinGame", playerName);
                Debug.Log($"[NetworkManager] JoinGame çağrıldı. playerName={playerName}");
            }
            else
            {
                Debug.LogError($"[NetworkManager] StartAsync sonrası state={connection.State}, JoinGame ÇAĞRILMADI.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[NetworkManager] Start / Join sırasında hata: {ex.Message}");
        }
    }

    // --- REMOTE PLAYER HAREKETİ ---
    private void OnRemotePlayerMoved(MoveState data)
    {
        // Kendi hareketimizi tekrar çizme
        if (data.playerName == playerName)
            return;

        if (otherPlayers.TryGetValue(data.playerName, out var enemy))
        {
            enemy.transform.position = new Vector3(data.x, data.y, 0);
        }
        else
        {
            if (remotePlayerPrefab == null)
            {
                Debug.LogWarning("[CLIENT] remotePlayerPrefab atanmadı!");
                return;
            }

            GameObject newEnemy =
                Instantiate(remotePlayerPrefab, new Vector3(data.x, data.y, 0), Quaternion.identity);

            var renderer = newEnemy.GetComponent<SpriteRenderer>();
            if (renderer) renderer.sortingOrder = 20;

            otherPlayers.Add(data.playerName, newEnemy);
            Debug.Log("[CLIENT] Yeni enemy spawn edildi.");
        }
    }

    // --- REMOTE BOMB SPAWN ---
    private void OnRemoteBombPlaced(BombData bomb)
    {
        if (remoteBombPrefab == null)
        {
            Debug.LogWarning("[CLIENT] remoteBombPrefab atanmadı!");
            return;
        }

        // Aynı bombId ile zaten varsa dokunma
        if (!string.IsNullOrEmpty(bomb.bombId) && remoteBombs.ContainsKey(bomb.bombId))
            return;

        Vector3 pos = new Vector3(bomb.x, bomb.y, 0);
        GameObject bombGO = Instantiate(remoteBombPrefab, pos, Quaternion.identity);

        if (!string.IsNullOrEmpty(bomb.bombId))
        {
            remoteBombs[bomb.bombId] = bombGO;
        }

        Debug.Log("[CLIENT] Remote bomba spawn edildi.");
    }

    // --- REMOTE PATLAMA ---
    private void OnRemoteBombExploded(BombData bomb)
    {
        // 1) Bomba objesini sahneden kaldır (varsa)
        if (!string.IsNullOrEmpty(bomb.bombId) &&
            remoteBombs.TryGetValue(bomb.bombId, out var bombGO))
        {
            if (bombGO != null) Destroy(bombGO);
            remoteBombs.Remove(bomb.bombId);
        }

        // 2) Patlama efekti spawn et
        if (remoteExplosionPrefab != null)
        {
            Vector3 pos = new Vector3(bomb.x, bomb.y, 0);
            GameObject expl = Instantiate(remoteExplosionPrefab, pos, Quaternion.identity);
            Destroy(expl, 0.5f); // kısa süre sonra yok et
        }

        Debug.Log("[CLIENT] Remote patlama efekti oynatıldı.");
    }

    // --- PLAYER ÖLÜMÜ ---
    private void OnPlayerDied(string deadId)
    {
        // Remote player öldüyse: kırmızı kareyi yok et
        if (otherPlayers.TryGetValue(deadId, out var enemy))
        {
            if (enemy != null) Destroy(enemy);
            otherPlayers.Remove(deadId);
            Debug.Log("[CLIENT] Remote player öldü, GameObject yok edildi.");
        }

        // Bu client öldüyse
        if (connection != null && deadId == connection.ConnectionId)
        {
            Debug.Log("[CLIENT] BU PLAYER ÖLDÜ (buraya GameOver / respawn bağlanabilir).");
        }
    }

    // --- GAME RESULT (WIN/LOSE) + LEADERBOARD ---
    public async void SendGameResult(bool didWin)
    {
        Debug.Log($"[NetworkManager] SendGameResult called. didWin={didWin}");

        // Bağlantı yoksa: sadece lokal paneli göster
        if (connection == null || connection.State != HubConnectionState.Connected)
        {
            Debug.LogWarning("[NetworkManager] SendGameResult skipped: not connected to server.");
            if (gameOverUI != null)
            {
                gameOverUI.Show(didWin, new List<LeaderboardEntry>());
            }
            return;
        }

        try
        {
            // 1) Sonucu sunucuya gönder
            await connection.InvokeAsync("SubmitGameResult", playerName, didWin);
            Debug.Log("[NetworkManager] GameResult sent to server.");

            // 2) Leaderboard iste
            List<LeaderboardEntry> leaderboard;
            try
            {
                leaderboard = await connection
                    .InvokeAsync<List<LeaderboardEntry>>("GetLeaderboard", 5);
                Debug.Log($"[NetworkManager] Leaderboard received. count={leaderboard.Count}");
            }
            catch (Exception ex)
            {
                Debug.LogError("[NetworkManager] GetLeaderboard failed: " + ex.Message);
                leaderboard = new List<LeaderboardEntry>();
            }

            // 3) UI’yi aç
            if (gameOverUI != null)
            {
                gameOverUI.Show(didWin, leaderboard);
            }
            else
            {
                Debug.LogWarning("[NetworkManager] GameOverUI reference is null.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[NetworkManager] SendGameResult error: " + ex);
            // hata olsa bile lokal paneli göstermeyi dene
            if (gameOverUI != null)
            {
                gameOverUI.Show(didWin, new List<LeaderboardEntry>());
            }
        }
    }

    // PlayerMovement için bağlantıyı expose ediyoruz
    public HubConnection GetConnection()
    {
        return connection;
    }
}
