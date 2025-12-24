using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public void Die()
    {
        Debug.Log("ENEMY DIED");
        gameObject.SetActive(false);
    }
}