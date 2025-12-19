using System.Collections.Generic;
using UnityEngine;

public class PlusExplosionPattern : IExplosionPattern
{
    public IEnumerable<Vector2Int> GetCells(Vector2Int center, int range, IWallQuery walls)
    {
        yield return center;

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (var dir in dirs)
        {
            var cur = center;
            for (int i = 0; i < range; i++)
            {
                cur += dir;

                // UNBREAKABLE: bu hücreye patlama çizme bile istemiyorsan yield etmeden dur.
                if (walls != null && walls.IsHard(cur))
                    break;

                // REINFORCED: hücreye "hit" gelsin, sonra dur
                if (walls != null && walls.IsReinforced(cur))
                {
                    yield return cur;
                    break;
                }

                // SOFT: hücreye patlama gelir, sonra durur
                if (walls != null && walls.IsSoft(cur))
                {
                    yield return cur;
                    break;
                }

                yield return cur;
            }
        }
    }
}