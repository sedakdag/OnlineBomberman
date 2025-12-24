using UnityEngine;

public class SoftWallSystem : MonoBehaviour
{
    [SerializeField] private TilemapManager tilemapManager;

    private void OnEnable()
    {
        GameEvents.ExplosionAtCell += OnExplosionAtCell;
    }

    private void OnDisable()
    {
        GameEvents.ExplosionAtCell -= OnExplosionAtCell;
    }

    private void OnExplosionAtCell(Vector2Int cell)
    {
        if (tilemapManager == null) return;

        if (!tilemapManager.IsSoftWall(cell))
            return;

        // Duvarı sil
        tilemapManager.ClearSoftWall(cell);

        // 🔥 Event fırlat
        GameEvents.RaiseSoftWallDestroyed(cell);
    }
}