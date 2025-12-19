using UnityEngine;

public class PooledExplosionFactory : IExplosionFactory
{
    private readonly GameObjectPool pool;
    private readonly float lifeSeconds;
    private readonly MonoBehaviour runner;

    public PooledExplosionFactory(MonoBehaviour runner, GameObject prefab, Transform parent, float lifeSeconds, int prewarm = 16)
    {
        this.runner = runner;
        this.lifeSeconds = lifeSeconds;
        pool = new GameObjectPool(prefab, parent, prewarm);
    }

    public GameObject Spawn(Vector3 worldPos, Transform parent)
    {
        var go = pool.Get();
        go.transform.position = worldPos;
        go.transform.rotation = Quaternion.identity;

        // otomatik release
        runner.StartCoroutine(ReleaseAfter(go));
        return go;
    }

    private System.Collections.IEnumerator ReleaseAfter(GameObject go)
    {
        yield return new WaitForSeconds(lifeSeconds);
        pool.Release(go);
    }
}