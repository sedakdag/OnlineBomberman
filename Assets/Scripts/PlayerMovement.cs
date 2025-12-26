using System;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using Contracts; // MoveState, BombData

public class PlayerMovement : MonoBehaviour
{
    private TilemapManager tilemapManager;

    private Vector3 targetPos;
    private bool isMoving;
    public float speed = 5f;

    // Player adı (Her window’da farklı vermelisin: Unity_Test_Oyuncusu, Unity_Test_Oyuncusu_2 vs.)
    [SerializeField] private string playerName = "Unity_Test_Oyuncusu";

    private void Start()
    {
        tilemapManager = FindFirstObjectByType<TilemapManager>();

        if (tilemapManager == null)
        {
            Debug.LogError("[CLIENT] TilemapManager bulunamadı.");
            return;
        }

        // Başlangıç pozisyonunu grid merkezine oturt
        Vector2Int startGridPos = tilemapManager.WorldToGrid(transform.position);
        transform.position = tilemapManager.GridToWorldCenter(startGridPos);
        targetPos = transform.position;
    }

    private void Update()
    {
        // Halihazırda hareket halindeysek interpolation devam etsin
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPos) < 0.001f)
            {
                transform.position = targetPos;
                isMoving = false;
            }
            return;
        }

        // Input oku
        Vector2Int dir = Vector2Int.zero;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            dir = Vector2Int.left;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            dir = Vector2Int.right;
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            dir = Vector2Int.up;
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            dir = Vector2Int.down;

        if (dir != Vector2Int.zero)
        {
            TryMove(dir);
        }

        // --- BOMBA ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("[CLIENT] SPACE basıldı, PlaceBomb() çağrılacak.");
            PlaceBomb();
        }
    }

    private void TryMove(Vector2Int dir)
    {
        if (tilemapManager == null)
            return;

        Vector2Int currentGridPos = tilemapManager.WorldToGrid(transform.position);
        Vector2Int nextGridPos = currentGridPos + dir;

        if (tilemapManager.IsBlocked(nextGridPos))
            return;

        // Yeni hedef dünya pozisyonu
        targetPos = tilemapManager.GridToWorldCenter(nextGridPos);
        isMoving = true;

        // Server’a hareket event’i yolla
        SendMovement(targetPos, dir);
    }

    // MoveState.direction için int mapping:
    // 0=left, 1=right, 2=up, 3=down, -1=none
    private int DirToInt(Vector2Int dir)
    {
        if (dir == Vector2Int.left)  return 0;
        if (dir == Vector2Int.right) return 1;
        if (dir == Vector2Int.up)    return 2;
        if (dir == Vector2Int.down)  return 3;
        return -1;
    }

    private async void SendMovement(Vector3 worldPos, Vector2Int dir)
    {
        var nm = NetworkManager.Instance;
        if (nm == null)
        {
            Debug.LogWarning("[CLIENT] SendMovement: NetworkManager.Instance yok.");
            return;
        }

        HubConnection conn = nm.GetConnection();
        if (conn == null)
        {
            Debug.LogWarning("[CLIENT] SendMovement: HubConnection NULL.");
            return;
        }

        if (conn.State != HubConnectionState.Connected)
        {
            Debug.LogWarning("[CLIENT] SendMovement: connection state=" + conn.State);
            return;
        }

        var moveState = new MoveState
        {
            playerName = playerName,
            x = worldPos.x,
            y = worldPos.y,
            direction = DirToInt(dir)
        };

        try
        {
            await conn.InvokeAsync("SendMove", moveState);
            // Debug.Log($"[CLIENT] SendMove yollandı: {moveState.playerName} -> ({moveState.x},{moveState.y}) dir={moveState.direction}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CLIENT] SendMove hata: {ex.Message}");
        }
    }

    private void PlaceBomb()
    {
        if (tilemapManager == null)
        {
            Debug.LogWarning("[CLIENT] PlaceBomb: tilemapManager yok.");
            return;
        }

        Vector2Int gridPos = tilemapManager.WorldToGrid(transform.position);
        SendBombToServer(gridPos);
    }

    private async void SendBombToServer(Vector2Int gridPos)
    {
        var nm = NetworkManager.Instance;
        if (nm == null)
        {
            Debug.LogWarning("[CLIENT] SendBombToServer: NetworkManager.Instance yok.");
            return;
        }

        HubConnection conn = nm.GetConnection();
        if (conn == null)
        {
            Debug.LogWarning("[CLIENT] SendBombToServer: HubConnection NULL.");
            return;
        }

        Debug.Log("[CLIENT] SendBombToServer: conn.State=" + conn.State);

        // Bomb world position (grid merkezine)
        Vector3 bombWorldPos = tilemapManager.GridToWorldCenter(gridPos);
        int power = 1;

        var bomb = new BombData
        {
            bombId = Guid.NewGuid().ToString(),
            ownerId = playerName,
            x = bombWorldPos.x,
            y = bombWorldPos.y,
            power = power,
            fuseTimeMs = 0f // şimdilik kullanmıyorsan 0
        };

        try
        {
            await conn.InvokeAsync("PlaceBomb", bomb);
            Debug.Log($"[CLIENT] PlaceBomb server'a gönderildi: id={bomb.bombId}, owner={bomb.ownerId}, pos=({bomb.x},{bomb.y}), power={bomb.power}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CLIENT] PlaceBomb hata: {ex.Message}");
        }
    }
}
