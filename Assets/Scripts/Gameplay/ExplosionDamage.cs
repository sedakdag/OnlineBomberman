using System.Collections;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [SerializeField] private float armDelay = 0.05f;

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
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
        {
            if (h.CompareTag("Player"))
            {
                h.GetComponent<PlayerHealth>()?.Die();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.Die();
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyHealth>()?.Die();
        }
    }
}