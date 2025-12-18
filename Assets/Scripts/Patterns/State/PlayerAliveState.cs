public class PlayerAliveState : IPlayerState
{
    public void Enter(PlayerStateMachine ctx)
    {
        if (ctx.Controller != null) ctx.Controller.SetInputEnabled(true);
    }

    public void Exit(PlayerStateMachine ctx) { }
}