using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private bool _isDead = false;   // aynı anda 2 kere tetiklenmesin diye

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        Debug.Log("ENEMY DIED");

        // 🔹 Düşman öldü → oyuncu kazandı
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.SendGameResult(true);   // WIN
        }

        // Eski davranış: sahneden kaybolsun
        gameObject.SetActive(false);
    }
}
