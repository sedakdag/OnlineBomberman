using UnityEngine;

public interface IExplosionFactory
{
    GameObject Spawn(Vector3 worldPos, Transform parent);
}