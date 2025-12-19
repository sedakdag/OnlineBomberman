using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStateMachine _sm;

    private void Awake()
    {
        _sm = GetComponent<PlayerStateMachine>();
    }

    public void Die()
    {
        Debug.Log("PLAYER DIED");
        if (_sm != null)
            _sm.ChangeState(new PlayerDeadState());
    }
}