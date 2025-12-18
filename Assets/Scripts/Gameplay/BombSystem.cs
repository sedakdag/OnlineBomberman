using System.Collections;
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
    [SerializeField] private int explosionRange = 2;

    private readonly HashSet<Vector2Int> activeBombCells = new();

    public bool TryPlaceBombAtWorld(Vector3 worldPos, Collider2D playerCol)
    {
        Vector2Int cell = tilemapManager.WorldToGrid(worldPos);

        if (tilemapManager.IsBlocked(cell)) return false;
        if (activeBombCells.Contains(cell)) return false;

        activeBombCells.Add(cell);

        Vector3 spawnPos = tilemapManager.GridToWorldCenter(cell);
        var bombObj = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        // Pass-through (klasik bomberman)
        var pass = bombObj.GetComponent<BombPassThrough>();
        if (pass != null && playerCol != null)
            pass.Init(playerCol);

        StartCoroutine(ExplodeAfterDelay(cell, fuseSeconds, bombObj));
        return true;
    }

    private IEnumerator ExplodeAfterDelay(Vector2Int cell, float delay, GameObject bombObj)
    {
        yield return new WaitForSeconds(delay);

        if (bombObj != null) Destroy(bombObj);

        SpawnExplosionPlus(cell);

        activeBombCells.Remove(cell);
    }

    private void SpawnExplosionPlus(Vector2Int center)
    {
        SpawnExplosionCell(center);

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in dirs)
        {
            Vector2Int cur = center;
            for (int i = 0; i < explosionRange; i++)
            {
                cur += dir;

                if (tilemapManager.IsBlocked(cur))
                    break;

                SpawnExplosionCell(cur);
            }
        }
    }

    private void SpawnExplosionCell(Vector2Int cell)
    {
        Vector3 pos = tilemapManager.GridToWorldCenter(cell);

        var go = Instantiate(explosionPrefab, pos, Quaternion.identity,
            explosionParent != null ? explosionParent : null);

        Destroy(go, 0.35f);
    }
    
    public bool IsBombCell(Vector2Int cell)
    {
        return activeBombCells.Contains(cell);
    }

}
