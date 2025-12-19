using UnityEngine;

public class PrefabExplosionFactory : IExplosionFactory
{
    private readonly GameObject _prefab;

    public PrefabExplosionFactory(GameObject prefab)
    {
        _prefab = prefab;
    }

    public GameObject Spawn(Vector3 worldPos, Transform parent)
    {
        return Object.Instantiate(_prefab, worldPos, Quaternion.identity, parent);
    }
}