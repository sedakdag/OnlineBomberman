using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ReinforcedWallSystem : MonoBehaviour
{
    [SerializeField] private Tilemap reinforcedWallTilemap;
    [SerializeField] private int hitsToBreak = 3;

    // damageTiles[0]=full, [1]=crack1, [2]=crack2 ...
    [SerializeField] private TileBase[] damageTiles;
    [SerializeField] private PlayerPowerStats playerStats;

    private readonly Dictionary<Vector3Int, int> hp = new();

    private void OnEnable()
    {
        GameEvents.ExplosionAtCellWithPower += OnExplosionAtCell;
    }

    private void OnDisable()
    {
        GameEvents.ExplosionAtCellWithPower -= OnExplosionAtCell;
    }


    private void OnExplosionAtCell(Vector2Int cell2, bool strongBomb)
    {
        if (reinforcedWallTilemap == null) return;

        var cell = new Vector3Int(cell2.x, cell2.y, 0);

        if (!reinforcedWallTilemap.HasTile(cell))
            return;

        if (!hp.TryGetValue(cell, out int curHp))
            curHp = hitsToBreak;   // ilk kez vuruluyorsa full HP ile başla

        if (strongBomb)
            curHp = 0;
        else
            curHp--;

        // 0 ve altı => kırıldı, sil
        if (curHp <= 0)
        {
            reinforcedWallTilemap.SetTile(cell, null);
            hp.Remove(cell);
            return;
        }

        hp[cell] = curHp;

        // Görsel güncelleme: hitsToBreak=3 iken
        // 1. hit sonrası curHp=2 => damageIndex=1 (crack1)
        // 2. hit sonrası curHp=1 => damageIndex=2 (crack2)
        int damageIndex = hitsToBreak - curHp;

        if (damageTiles != null && damageTiles.Length > 0)
        {
            damageIndex = Mathf.Clamp(damageIndex, 0, damageTiles.Length - 1);
            reinforcedWallTilemap.SetTile(cell, damageTiles[damageIndex]);
        }
    }
}