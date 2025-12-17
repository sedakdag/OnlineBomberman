using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private BombSystem bombSystem;

    private Rigidbody2D rb;
    private Vector2Int gridPos;
    private Vector2 targetPos;
    private bool isMoving;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // RoundToInt YOK: Tilemap/Grid dönüşümü var
        gridPos = tilemapManager.WorldToGrid(transform.position);
        targetPos = tilemapManager.GridToWorldCenter(gridPos);
        rb.position = targetPos;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bombSystem.TryPlaceBombAtWorld(transform.position);
        }

        if (isMoving) return;

        Vector2Int dir = ReadInput();
        if (dir != Vector2Int.zero)
            TryMove(dir);
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

        if (tilemapManager != null && tilemapManager.IsBlocked(next))
            return;

        gridPos = next;
        targetPos = tilemapManager.GridToWorldCenter(gridPos);
        isMoving = true;
    }
}