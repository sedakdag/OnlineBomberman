using UnityEngine;

public class PlayerDeadState : IPlayerState
{
    public void Enter(PlayerStateMachine ctx)
    {
        if (ctx.Controller != null) ctx.Controller.SetInputEnabled(false);

        // Basit “ölüm”: görünmez + collider kapat
        var sr = ctx.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        var col = ctx.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Debug.Log("PLAYER STATE = DEAD");
    }

    public void Exit(PlayerStateMachine ctx) { }
}