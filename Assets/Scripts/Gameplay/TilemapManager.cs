using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap wallTilemap;

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        var cell = grid.WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.y);
    }

    public Vector3 GridToWorldCenter(Vector2Int gridPos)
    {
        var cell = new Vector3Int(gridPos.x, gridPos.y, 0);
        // Hücrenin merkezine oturt
        return grid.GetCellCenterWorld(cell);
    }

    public bool IsBlocked(Vector2Int gridPos)
    {
        if (wallTilemap == null) return false;
        var cell = new Vector3Int(gridPos.x, gridPos.y, 0);
        return wallTilemap.HasTile(cell);
    }
}