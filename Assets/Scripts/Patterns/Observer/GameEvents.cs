using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<Vector2Int> ExplosionAtCell;

    public static void RaiseExplosionAtCell(Vector2Int cell)
        => ExplosionAtCell?.Invoke(cell);
}