using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [SerializeField] private Grid grid;

    [Header("Walls")]
    [SerializeField] private Tilemap hardWallTilemap; // kırılmaz
    [SerializeField] private Tilemap softWallTilemap; // kırılabilir
    [SerializeField] private Tilemap reinforcedWallTilemap; 

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        var cell = grid.WorldToCell(worldPos);
        return new Vector2Int(cell.x, cell.y);
    }

    public Vector3 GridToWorldCenter(Vector2Int gridPos)
    {
        var cell = new Vector3Int(gridPos.x, gridPos.y, 0);
        return grid.GetCellCenterWorld(cell);
    }

    public bool IsHardWall(Vector2Int p)
    {
        var c = new Vector3Int(p.x, p.y, 0);
        return hardWallTilemap != null && hardWallTilemap.HasTile(c);
    }

    public bool IsSoftWall(Vector2Int p)
    {
        var c = new Vector3Int(p.x, p.y, 0);
        return softWallTilemap != null && softWallTilemap.HasTile(c);
    }

    public bool IsReinforcedWall(Vector2Int p)
    {
        var c = new Vector3Int(p.x, p.y, 0);
        return reinforcedWallTilemap != null && reinforcedWallTilemap.HasTile(c);
    }

    // Player hareketi için hepsi bloklasın
    public bool IsBlocked(Vector2Int p)
    {
        return IsHardWall(p) || IsSoftWall(p) || IsReinforcedWall(p);
    }

    public void ClearSoftWall(Vector2Int gridPos)
    {
        if (softWallTilemap == null) return;
        var cell = new Vector3Int(gridPos.x, gridPos.y, 0);
        softWallTilemap.SetTile(cell, null);
    }
    
}