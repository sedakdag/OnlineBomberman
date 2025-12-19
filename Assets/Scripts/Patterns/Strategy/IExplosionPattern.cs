using System.Collections.Generic;
using UnityEngine;

public interface IExplosionPattern
{
    IEnumerable<Vector2Int> GetCells(Vector2Int center, int range, IWallQuery walls);
}