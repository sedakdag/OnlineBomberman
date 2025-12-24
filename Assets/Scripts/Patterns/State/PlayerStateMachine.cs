using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    public PlayerController Controller { get; private set; }
    public PlayerHealth Health { get; private set; }

    private IPlayerState _state;

    public bool IsDead => _state is PlayerDeadState;

    private void Awake()
    {
        Controller = GetComponent<PlayerController>();
        Health = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        ChangeState(new PlayerAliveState());
    }

    public void ChangeState(IPlayerState next)
    {
        _state?.Exit(this);
        _state = next;
        _state?.Enter(this);
    }
}
