using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyKillOnTouch : MonoBehaviour
{
    private void Reset()
    {
        // Kolaylık: Enemy collider'ı trigger yap
        var c = GetComponent<Collider2D>();
        c.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player’ı bul
        var health = other.GetComponent<PlayerHealth>();
        if (health == null) health = other.GetComponentInParent<PlayerHealth>();

        if (health == null) return;

        // Zaten dead ise spam yapma (isteğe bağlı)
        var sm = health.GetComponent<PlayerStateMachine>();
        if (sm != null && sm.IsDead) return;

        health.Die();
    }
}