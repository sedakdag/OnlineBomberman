using System.Collections;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [SerializeField] private float armDelay = 0.05f;
    [SerializeField] private int playerDamage = 1;

    private Collider2D col;

    // ✅ aynı patlamada aynı hedefe 1 kez vur
    private bool _hitPlayer;
    private bool _hitEnemy;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        _hitPlayer = false;
        _hitEnemy = false;
        StartCoroutine(ArmAndCheck());
    }

    private IEnumerator ArmAndCheck()
    {
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(armDelay);

        if (col != null) col.enabled = true;

        // Spawn anında üstündeyse de yakala
        var hits = Physics2D.OverlapBoxAll(col.bounds.center, col.bounds.size, 0f);
        foreach (var h in hits)
            Apply(h);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Apply(other);
    }

    private void Apply(Collider2D other)
    {
        if (other == null) return;

        if (other.CompareTag("Player"))
        {
            if (_hitPlayer) return;        // ✅ ikinciyi engelle
            _hitPlayer = true;

            var health = other.GetComponent<PlayerHealth>() ?? other.GetComponentInParent<PlayerHealth>();
            if (health != null) health.TakeDamage(playerDamage);
        }
        else if (other.CompareTag("Enemy"))
        {
            if (_hitEnemy) return;
            _hitEnemy = true;

            var e = other.GetComponent<EnemyHealth>() ?? other.GetComponentInParent<EnemyHealth>();
            if (e != null) e.Die();
        }
    }
}