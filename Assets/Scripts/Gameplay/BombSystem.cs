using System.Collections.Generic;
using UnityEngine;

public class BombSystem : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TilemapManager tilemapManager;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private Transform explosionParent;

    [Header("Bomb Params")]
    [SerializeField] private float fuseSeconds = 2f;
    [SerializeField] private int explosionRange = 2; // kaç hücre gidecek

    // aynı hücreye 2 bomba koymayı engelle
    private readonly HashSet<Vector2Int> activeBombCells = new();

    public bool TryPlaceBombAtWorld(Vector3 worldPos)
    {
        Vector2Int cell = tilemapManager.WorldToGrid(worldPos);

        // duvarın üstüne koyma
        if (tilemapManager.IsBlocked(cell)) return false;

        // aynı hücreye tekrar koyma
        if (activeBombCells.Contains(cell)) return false;

        activeBombCells.Add(cell);

        Vector3 spawnPos = tilemapManager.GridToWorldCenter(cell);
        GameObject bomb = Instantiate(explosionPrefab, spawnPos, Quaternion.identity, explosionParent);


        StartCoroutine(ExplodeAfterDelay(cell, fuseSeconds, bomb));
        return true;
    }

    private System.Collections.IEnumerator ExplodeAfterDelay(Vector2Int cell, float delay, GameObject bombObj)
    {
        yield return new WaitForSeconds(delay);

        if (bombObj != null) Destroy(bombObj);

        SpawnExplosionPlus(cell);

        activeBombCells.Remove(cell);
    }

    private void SpawnExplosionPlus(Vector2Int center)
    {
        // merkez
        SpawnExplosionCell(center);

        // 4 yön
        Vector2Int[] dirs = {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in dirs)
        {
            Vector2Int cur = center;
            for (int i = 0; i < explosionRange; i++)
            {
                cur += dir;

                // duvar varsa patlama o yönde durur (şimdilik kırma yok)
                if (tilemapManager.IsBlocked(cur))
                    break;

                SpawnExplosionCell(cur);
            }
        }
    }

    private void SpawnExplosionCell(Vector2Int cell)
    {
        Vector3 pos = tilemapManager.GridToWorldCenter(cell);
        var go = Instantiate(explosionPrefab, pos, Quaternion.identity);
        Destroy(go, 0.35f); // patlama kısa sürsün
    }
}
