public interface IPlayerState
{
    void Enter(PlayerStateMachine ctx);
    void Exit(PlayerStateMachine ctx);
}