using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public void Die()
    {
        Debug.Log("PLAYER DIED");
        gameObject.SetActive(false); // şimdilik böyle
    }
}