using UnityEngine;

public class PowerUpFactory : MonoBehaviour
{
    [System.Serializable]
    public struct Entry
    {
        public PowerUpType type;
        public GameObject prefab;
    }

    [SerializeField] private Entry[] entries;
    [SerializeField] private Transform parent;

    public GameObject Spawn(PowerUpType type, Vector3 pos)
    {
        foreach (var e in entries)
        {
            if (e.type == type && e.prefab != null)
                return Instantiate(e.prefab, pos, Quaternion.identity, parent);
        }
        return null;
    }
}