using UnityEngine;

public interface IWallQuery
{
    bool IsHard(Vector2Int cell);        // unbreakable
    bool IsSoft(Vector2Int cell);        // breakable
    bool IsReinforced(Vector2Int cell);  // multi-hit hard
}
