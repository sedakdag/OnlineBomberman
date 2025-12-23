using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private PowerUpFactory factory;

    [Range(0f, 1f)]
    [SerializeField] private float spawnChance = 0.35f;

    private void OnEnable()
    {
        GameEvents.SoftWallDestroyed += OnSoftWallDestroyed;
    }

    private void OnDisable()
    {
        GameEvents.SoftWallDestroyed -= OnSoftWallDestroyed;
    }

    private void OnSoftWallDestroyed(Vector2Int cell)
    {
        if (Random.value > spawnChance) return;

        PowerUpType type = (PowerUpType)Random.Range(0, 3);

        Vector3 worldPos = tilemapManager.GridToWorldCenter(cell);
        factory.Spawn(type, worldPos);

        GameEvents.RaisePowerUpSpawned(cell, type);
    }
}
