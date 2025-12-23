using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<Vector2Int> ExplosionAtCell;
    public static event Action<Vector2Int, bool> ExplosionAtCellWithPower;

    public static void RaiseExplosionAtCell(Vector2Int cell, bool strong)
    {
        ExplosionAtCellWithPower?.Invoke(cell, strong);
    }
    
    public static void RaiseExplosionAtCell(Vector2Int cell)
        => ExplosionAtCell?.Invoke(cell);
    
    // 🔹 Soft wall kırıldı
    public static event Action<Vector2Int> SoftWallDestroyed;
    public static void RaiseSoftWallDestroyed(Vector2Int cell)
        => SoftWallDestroyed?.Invoke(cell);

    // 🔹 PowerUp spawn oldu (debug / network / UI için)
    public static event Action<Vector2Int, PowerUpType> PowerUpSpawned;
    public static void RaisePowerUpSpawned(Vector2Int cell, PowerUpType type)
        => PowerUpSpawned?.Invoke(cell, type);
    
}