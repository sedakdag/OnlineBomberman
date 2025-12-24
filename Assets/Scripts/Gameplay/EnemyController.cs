using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private BombSystem bombSystem;
    [SerializeField] private Transform player;

    [Header("Move")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float repathInterval = 0.25f;

    [Header("Bomb AI")]
    [SerializeField] private int placeBombIfWithinCells = 1; // player çok yakınsa
    [SerializeField] private float bombCooldown = 1.2f;

    private Rigidbody2D rb;
    private Vector2Int gridPos;
    private Vector2 targetPos;
    private bool isMoving;

    private float repathTimer;
    private float bombTimer;

    private IEnemyState state;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        gridPos = tilemapManager.WorldToGrid(transform.position);
        targetPos = tilemapManager.GridToWorldCenter(gridPos);
        rb.position = targetPos;

        state = new EnemyChaseState(); // başlangıç
        state.Enter(this);
    }

    void Update()
    {
        bombTimer -= Time.deltaTime;

        // ✅ hareket halindeyken state logic'i yürütme (özellikle MoveStep)
        if (isMoving) return;

        // state update (artık sadece idle iken)
        state.Tick(this);

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            state.Repath(this);
        }
    }

    void FixedUpdate()
    {
        if (!isMoving) return;

        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector2.Distance(rb.position, targetPos) < 0.01f)
        {
            rb.MovePosition(targetPos);
            isMoving = false;
        }
    }

    // === API for states ===
    public Vector2Int GridPos => gridPos;
    public Vector2 PlayerWorld => player.position;
    public Vector2Int PlayerCell => tilemapManager.WorldToGrid(player.position);
    public bool CanPlaceBomb => bombTimer <= 0f;

    public bool IsBlocked(Vector2Int cell) => tilemapManager.IsBlocked(cell) || bombSystem.IsCellOccupiedByBomb(cell);

    public bool IsDanger(Vector2Int cell) => bombSystem.IsCellDangerous(cell);

    public void MoveStep(Vector2Int nextCell)
    {
        Debug.Log($"Enemy MoveStep {gridPos} -> {nextCell} blocked={IsBlocked(nextCell)}");

        if (IsBlocked(nextCell)) return;

        gridPos = nextCell;
        targetPos = tilemapManager.GridToWorldCenter(gridPos);
        isMoving = true;
    }

    public bool TryPlaceBomb()
    {
        if (!CanPlaceBomb) return false;

        var myCol = GetComponent<Collider2D>();
        bool ok = bombSystem.TryPlaceBombAtWorld(transform.position, myCol);
        if (ok) bombTimer = bombCooldown;
        return ok;
    }

    public void SwitchState(IEnemyState next)
    {
        if (state == next) return;
        state.Exit(this);
        state = next;
        state.Enter(this);
    }
}
