using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int maxHP = 3;
    public int CurrentHP { get; private set; }

    [Header("Refs")]
    [SerializeField] private HUDController hud; // HUD_Root üstündeki HUDController'ı buraya bağla

    private PlayerStateMachine _sm;

    private void Awake()
    {
        _sm = GetComponent<PlayerStateMachine>();
        CurrentHP = maxHP;
    }

    private void Start()
    {
        // oyun başında kalpleri doğru çizsin
        if (hud != null) hud.RefreshHearts();
    }

    /// <summary>
    /// Patlama, enemy temas vb. yerlerden çağır.
    /// </summary>
    public void TakeDamage(int amount = 1)
    {
        if (CurrentHP <= 0) return;

        CurrentHP = Mathf.Max(0, CurrentHP - amount);

        if (hud != null) hud.RefreshHearts();

        if (CurrentHP <= 0)
            Die();
    }

    public void Heal(int amount = 1)
    {
        if (CurrentHP <= 0) return;

        CurrentHP = Mathf.Clamp(CurrentHP + amount, 0, maxHP);

        if (hud != null) hud.RefreshHearts();
    }

    public void Die()
    {
        Debug.Log("PLAYER DIED");

        // HUD panel göster (player objesini kapatmadan önce)
        if (hud != null) hud.ShowGameOver();

        // State machine ile dead state
        if (_sm != null)
            _sm.ChangeState(new PlayerDeadState());
    }
}