using System;
using System.Collections.Generic;
using UnityEngine;

public class PlusExplosionPattern : IExplosionPattern
{
    public IEnumerable<Vector2Int> GetCells(Vector2Int center, int range, Func<Vector2Int, bool> isBlocked)
    {
        yield return center;

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in dirs)
        {
            var cur = center;
            for (int i = 0; i < range; i++)
            {
                cur += dir;
                if (isBlocked(cur)) break;
                yield return cur;
            }
        }
    }
}