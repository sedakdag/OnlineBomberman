using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private PlayerPowerStats _stats;
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private BombSystem bombSystem;

    [Header("Visuals (Facing Direction)")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [System.Serializable]
    public class DirectionSprites
    {
        public Sprite up;
        public Sprite down;
        public Sprite left;
        public Sprite right;
    }

    [SerializeField] private DirectionSprites directionSprites;

    private Rigidbody2D rb;
    private Vector2Int gridPos;
    private Vector2 targetPos;
    private bool isMoving;

    // Command pattern (bomb placing)
    private CommandInvoker _invoker;
    private Collider2D _playerCol;

    // Input enable/disable (for stun/death etc.)
    private bool _inputEnabled = true;
    public void SetInputEnabled(bool enabled) => _inputEnabled = enabled;

    // State pattern (facing direction)
    private interface IPlayerDirectionState
    {
        Vector2Int Dir { get; }
        void Apply(PlayerController ctx);
    }

    private class UpState : IPlayerDirectionState
    {
        public Vector2Int Dir => Vector2Int.up;
        public void Apply(PlayerController ctx)
        {
            if (ctx.spriteRenderer != null && ctx.directionSprites.up != null)
                ctx.spriteRenderer.sprite = ctx.directionSprites.up;
        }
    }

    private class DownState : IPlayerDirectionState
    {
        public Vector2Int Dir => Vector2Int.down;
        public void Apply(PlayerController ctx)
        {
            if (ctx.spriteRenderer != null && ctx.directionSprites.down != null)
                ctx.spriteRenderer.sprite = ctx.directionSprites.down;
        }
    }

    private class LeftState : IPlayerDirectionState
    {
        public Vector2Int Dir => Vector2Int.left;
        public void Apply(PlayerController ctx)
        {
            if (ctx.spriteRenderer != null && ctx.directionSprites.left != null)
                ctx.spriteRenderer.sprite = ctx.directionSprites.left;
        }
    }

    private class RightState : IPlayerDirectionState
    {
        public Vector2Int Dir => Vector2Int.right;
        public void Apply(PlayerController ctx)
        {
            if (ctx.spriteRenderer != null && ctx.directionSprites.right != null)
                ctx.spriteRenderer.sprite = ctx.directionSprites.right;
        }
    }

    private IPlayerDirectionState _up, _down, _left, _right;
    private IPlayerDirectionState _currentDirState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _stats = GetComponent<PlayerPowerStats>();

        _playerCol = GetComponent<Collider2D>();
        if (_playerCol == null) _playerCol = GetComponentInChildren<Collider2D>();

        _invoker = new CommandInvoker();

        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        // init direction states
        _up = new UpState();
        _down = new DownState();
        _left = new LeftState();
        _right = new RightState();

        // default facing
        _currentDirState = _down;
        _currentDirState.Apply(this);
    }

    void Start()
    {
        gridPos = tilemapManager.WorldToGrid(transform.position);
        targetPos = tilemapManager.GridToWorldCenter(gridPos);
        rb.position = targetPos;
    }

    void Update()
    {
        if (!_inputEnabled) return;

        // Command pattern for bomb placement
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var cmd = new PlaceBombCommand(bombSystem, transform, _playerCol, _stats);
            _invoker.Enqueue(cmd);
        }

        if (!isMoving)
        {
            Vector2Int dir = ReadInput();

            if (dir != Vector2Int.zero)
            {
                // rotate sprite immediately even if move fails
                SetDirectionState(dir);

                // try movement on grid
                TryMove(dir);
            }
        }

        _invoker.ExecuteAll();
    }

    void FixedUpdate()
    {
        if (!isMoving) return;

        float speed = _stats != null ? _stats.speed : 5f;

        Vector2 newPos = Vector2.MoveTowards(
            rb.position,
            targetPos,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPos);

        if (Vector2.Distance(rb.position, targetPos) < 0.01f)
        {
            rb.MovePosition(targetPos);
            isMoving = false;
        }
    }

    Vector2Int ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) return Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) return Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) return Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) return Vector2Int.right;
        return Vector2Int.zero;
    }

    void TryMove(Vector2Int dir)
    {
        Vector2Int next = gridPos + dir;

        // blocked by walls
        if (tilemapManager != null && tilemapManager.IsBlocked(next))
            return;

        // blocked by active bomb cell
        if (bombSystem != null && bombSystem.IsBombCell(next))
            return;

        gridPos = next;
        targetPos = tilemapManager.GridToWorldCenter(gridPos);
        isMoving = true;
    }

    private void SetDirectionState(Vector2Int dir)
    {
        IPlayerDirectionState next =
            dir == Vector2Int.up ? _up :
            dir == Vector2Int.down ? _down :
            dir == Vector2Int.left ? _left :
            dir == Vector2Int.right ? _right :
            null;

        if (next == null || next == _currentDirState) return;

        _currentDirState = next;
        _currentDirState.Apply(this);
    }
}
