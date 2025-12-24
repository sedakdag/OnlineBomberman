using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyKillOnTouch : MonoBehaviour
{
    [SerializeField] private int touchDamage = 1;
    [SerializeField] private float invincibilitySeconds = 0.5f;

    private void Reset()
    {
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var health = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
        if (health == null) return;

        var iframe = health.GetComponent<PlayerIFrames>();
        if (iframe == null) iframe = health.gameObject.AddComponent<PlayerIFrames>();

        if (!iframe.CanTakeDamage()) return;

        health.TakeDamage(touchDamage);
        iframe.Trigger(invincibilitySeconds);
    }
}

public class PlayerIFrames : MonoBehaviour
{
    private float _until;

    public bool CanTakeDamage() => Time.time >= _until;

    public void Trigger(float seconds)
    {
        _until = Time.time + seconds;
    }
}