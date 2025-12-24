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
    private PlayerPowerStats _ownerStats;
    private int _activeBombs;

    private void Awake()
    {
        _walls = new TilemapWallQuery(tilemapManager);
    }
    
    public bool TryPlaceBombAtWorld(Vector3 worldPos, Collider2D ownerCol, PlayerPowerStats stats = null)
    {
        _ownerStats = stats;

        // stats yoksa enemy gibi düşün: strongBomb=false, bombCount limiti kontrol etme
        bool strongBomb = (_ownerStats != null && _ownerStats.reinforcedOneHit);

        if (_ownerStats != null)
        {
            if (_activeBombs >= _ownerStats.bombCount)
                return false;
        }

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

        if (_ownerStats != null) _activeBombs++;

        Vector3 spawnPos = tilemapManager.GridToWorldCenter(cell);
        var bombObj = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        var pass = bombObj.GetComponent<BombPassThrough>();
        if (pass != null && ownerCol != null)
            pass.Init(ownerCol);

        StartCoroutine(ExplodeAfterDelay(cell, fuseSeconds, bombObj, strongBomb));
        return true;
    }


    private IEnumerator ExplodeAfterDelay(Vector2Int cell, float delay, GameObject bombObj, bool strongBomb)
    {
        yield return new WaitForSeconds(delay);

        if (bombObj != null) Destroy(bombObj);

        SpawnExplosion(cell, strongBomb);

        activeBombCells.Remove(cell);

        if (_ownerStats != null) _activeBombs--;
    }

    private void SpawnExplosion(Vector2Int centerCell, bool strongBomb)
    {
        foreach (var cur in _pattern.GetCells(centerCell, explosionRange, _walls))
        {
            SpawnExplosionCell(cur, strongBomb);
        }
    }

    private void SpawnExplosionCell(Vector2Int cell, bool strongBomb)
    {
        // eski event (soft wall vs.)
        GameEvents.RaiseExplosionAtCell(cell);

        // 🔥 YENİ: bombanın gücüyle birlikte
        GameEvents.RaiseExplosionAtCell(cell, strongBomb);

        Vector3 pos = tilemapManager.GridToWorldCenter(cell);
        _explosionFactory.Spawn(pos, explosionParent);
    }

    
    public bool IsBombCell(Vector2Int cell)
    {
        return activeBombCells.Contains(cell);
    }
    
    public bool IsCellOccupiedByBomb(Vector2Int cell) => activeBombCells.Contains(cell);
    public bool IsCellDangerous(Vector2Int cell)
    {
        // Her aktif bombanın patlama hücrelerini üretip bu cell var mı bak
        foreach (var bombCell in activeBombCells)
        {
            foreach (var c in _pattern.GetCells(bombCell, explosionRange, _walls))
            {
                if (c == cell) return true;
            }
        }
        return false;
    }
}
