using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState
{
    void Enter(EnemyController e);
    void Exit(EnemyController e);
    void Tick(EnemyController e);
    void Repath(EnemyController e);
}

public class EnemyChaseState : IEnemyState
{
    private Queue<Vector2Int> path = new();

    public void Enter(EnemyController e) { }
    public void Exit(EnemyController e) { path.Clear(); }

    public void Tick(EnemyController e)
    {
        if (e.IsDanger(e.GridPos))
        {
            e.SwitchState(new EnemyEvadeState());
            return;
        }

        int manhattan = Mathf.Abs(e.GridPos.x - e.PlayerCell.x) + Mathf.Abs(e.GridPos.y - e.PlayerCell.y);
        if (manhattan <= 1 && e.CanPlaceBomb)
        {
            e.SwitchState(new EnemyPlaceBombState());
            return;
        }

        if (path.Count > 0)
            e.MoveStep(path.Dequeue());
    }

    public void Repath(EnemyController e)
    {
        path = SimpleBfs.FindPath(
            start: e.GridPos,
            goal: e.PlayerCell,
            isBlocked: e.IsBlocked,
            maxVisited: 600
        );
    }
}

public class EnemyEvadeState : IEnemyState
{
    private Queue<Vector2Int> path = new();

    public void Enter(EnemyController e) { }
    public void Exit(EnemyController e) { path.Clear(); }

    public void Tick(EnemyController e)
    {
        if (!e.IsDanger(e.GridPos))
        {
            e.SwitchState(new EnemyChaseState());
            return;
        }

        if (path.Count > 0)
            e.MoveStep(path.Dequeue());
    }

    public void Repath(EnemyController e)
    {
        // en yakın "safe" cell: BFS ile ilk danger olmayan hücre
        path = SimpleBfs.FindNearestSafe(
            start: e.GridPos,
            isBlocked: e.IsBlocked,
            isDanger: e.IsDanger,
            maxVisited: 800
        );
    }
}

public class EnemyPlaceBombState : IEnemyState
{
    private bool placed;

    public void Enter(EnemyController e) { placed = false; }
    public void Exit(EnemyController e) { }

    public void Tick(EnemyController e)
    {
        if (!placed)
        {
            placed = e.TryPlaceBomb();
        }

        // bombayı koyduktan sonra hemen kaç
        e.SwitchState(new EnemyEvadeState());
    }

    public void Repath(EnemyController e) { }
}