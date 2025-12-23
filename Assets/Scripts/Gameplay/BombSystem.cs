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
    
    private IExplosionPattern _pattern = new PlusExplosionPattern();
    private IExplosionFactory _explosionFactory;
    private IWallQuery _walls;

    private void Awake()
    {
        _walls = new TilemapWallQuery(tilemapManager);
    }
    
    public bool TryPlaceBombAtWorld(Vector3 worldPos, Collider2D playerCol)
    {
        _explosionFactory ??= new PooledExplosionFactory(
            runner: this,
            prefab: explosionPrefab,
            parent: explosionParent,
            lifeSeconds: 0.35f,
            prewarm: 24
        );

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

        SpawnExplosion(cell);

        activeBombCells.Remove(cell);
    }

    private void SpawnExplosion(Vector2Int centerCell)
    {
        foreach (var cur in _pattern.GetCells(centerCell, explosionRange, _walls))
        {
            SpawnExplosionCell(cur);
        }
    }


    private void SpawnExplosionCell(Vector2Int cell)
    {
        GameEvents.RaiseExplosionAtCell(cell);

        Vector3 pos = tilemapManager.GridToWorldCenter(cell);
        _explosionFactory.Spawn(pos, explosionParent);
    }
    
    public bool IsBombCell(Vector2Int cell)
    {
        return activeBombCells.Contains(cell);
    }

}
