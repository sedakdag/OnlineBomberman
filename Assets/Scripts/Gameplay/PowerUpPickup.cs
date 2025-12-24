using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    [SerializeField] private PowerUpType type;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var stats = other.GetComponentInParent<PlayerPowerStats>();
        if (stats == null) return;

        PowerUpEffectFactory.Create(type).Apply(stats);
        Destroy(gameObject);
    }
}