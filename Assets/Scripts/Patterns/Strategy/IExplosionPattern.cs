using System;
using System.Collections.Generic;
using UnityEngine;

public interface IExplosionPattern
{
    IEnumerable<Vector2Int> GetCells(Vector2Int center, int range, Func<Vector2Int, bool> isBlocked);
}