using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ReinforcedWallSystem : MonoBehaviour
{
    [SerializeField] private Tilemap reinforcedWallTilemap;
    [SerializeField] private int hitsToBreak = 3;

    // (opsiyonel) çatlak görselleri: index 0 = full, 1 = crack1, 2 = crack2 ...
    [SerializeField] private TileBase[] damageTiles;

    private readonly Dictionary<Vector3Int, int> hp = new();

    private void OnEnable()  => GameEvents.ExplosionAtCell += OnExplosionAtCell;
    private void OnDisable() => GameEvents.ExplosionAtCell -= OnExplosionAtCell;

    private void OnExplosionAtCell(Vector2Int cell2)
    {
        if (reinforcedWallTilemap == null) return;

        var cell = new Vector3Int(cell2.x, cell2.y, 0);

        if (!reinforcedWallTilemap.HasTile(cell))
            return;

        if (!hp.TryGetValue(cell, out int curHp))
            curHp = hitsToBreak;

        curHp--;

        if (curHp <= 0)
        {
            reinforcedWallTilemap.SetTile(cell, null);
            hp.Remove(cell);
            return;
        }

        hp[cell] = curHp;

        // opsiyonel: kalan hp’ye göre tile değiştir (çatlak)
        if (damageTiles != null && damageTiles.Length > 0)
        {
            // Örn: hitsToBreak=3 => hp 2 kaldıysa idx=1, hp 1 kaldıysa idx=2
            int damageIndex = Mathf.Clamp(hitsToBreak - curHp, 0, damageTiles.Length - 1);
            reinforcedWallTilemap.SetTile(cell, damageTiles[damageIndex]);
        }
    }
}
