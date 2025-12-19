using UnityEngine;

public class TilemapWallQuery : IWallQuery
{
    private readonly TilemapManager tm;

    public TilemapWallQuery(TilemapManager tm) => this.tm = tm;

    public bool IsHard(Vector2Int cell) => tm != null && tm.IsHardWall(cell);
    public bool IsSoft(Vector2Int cell) => tm != null && tm.IsSoftWall(cell);
    public bool IsReinforced(Vector2Int cell) => tm != null && tm.IsReinforcedWall(cell);
}