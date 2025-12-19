using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : IPool<GameObject>
{
    private readonly GameObject prefab;
    private readonly Transform parent;
    private readonly Stack<GameObject> stack = new();

    public GameObjectPool(GameObject prefab, Transform parent, int prewarm = 0)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < prewarm; i++)
        {
            var go = Object.Instantiate(prefab, parent);
            go.SetActive(false);
            stack.Push(go);
        }
    }

    public GameObject Get()
    {
        GameObject go = stack.Count > 0 ? stack.Pop() : Object.Instantiate(prefab, parent);
        go.SetActive(true);
        return go;
    }

    public void Release(GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);
        go.transform.SetParent(parent, false);
        stack.Push(go);
    }
}