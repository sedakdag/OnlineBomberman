using System;
using System.Collections.Generic;
using UnityEngine;

public static class SimpleBfs
{
    static readonly Vector2Int[] Dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public static Queue<Vector2Int> FindPath(Vector2Int start, Vector2Int goal, Func<Vector2Int, bool> isBlocked, int maxVisited)
    {
        var came = new Dictionary<Vector2Int, Vector2Int>();
        var q = new Queue<Vector2Int>();
        var visited = new HashSet<Vector2Int>();

        q.Enqueue(start);
        visited.Add(start);

        int steps = 0;
        while (q.Count > 0 && steps++ < maxVisited)
        {
            var cur = q.Dequeue();
            if (cur == goal) break;

            foreach (var d in Dirs)
            {
                var nx = cur + d;
                if (visited.Contains(nx)) continue;
                if (isBlocked(nx)) continue;

                visited.Add(nx);
                came[nx] = cur;
                q.Enqueue(nx);
            }
        }

        if (!came.ContainsKey(goal)) return new Queue<Vector2Int>();

        // reconstruct
        var stack = new Stack<Vector2Int>();
        var t = goal;
        while (t != start)
        {
            stack.Push(t);
            t = came[t];
        }

        return new Queue<Vector2Int>(stack);
    }

    public static Queue<Vector2Int> FindNearestSafe(Vector2Int start, Func<Vector2Int, bool> isBlocked, Func<Vector2Int, bool> isDanger, int maxVisited)
    {
        var came = new Dictionary<Vector2Int, Vector2Int>();
        var q = new Queue<Vector2Int>();
        var visited = new HashSet<Vector2Int>();

        q.Enqueue(start);
        visited.Add(start);

        Vector2Int found = start;
        bool hasFound = false;

        int steps = 0;
        while (q.Count > 0 && steps++ < maxVisited)
        {
            var cur = q.Dequeue();
            if (!isDanger(cur))
            {
                found = cur;
                hasFound = true;
                break;
            }

            foreach (var d in Dirs)
            {
                var nx = cur + d;
                if (visited.Contains(nx)) continue;
                if (isBlocked(nx)) continue;

                visited.Add(nx);
                came[nx] = cur;
                q.Enqueue(nx);
            }
        }

        if (!hasFound || found == start) return new Queue<Vector2Int>();

        var stack = new Stack<Vector2Int>();
        var t = found;
        while (t != start)
        {
            stack.Push(t);
            t = came[t];
        }

        return new Queue<Vector2Int>(stack);
    }
}
