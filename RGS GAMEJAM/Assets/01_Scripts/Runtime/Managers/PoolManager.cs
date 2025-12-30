using UnityEngine;
using Mirror;
using JamesFrowen.Spawning;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    [System.Serializable]
    public class PoolEntry
    {
        public GameObject prefab;
        public int capacity = 100;
        public int warmup = 20;
    }

    [Header("Pools")]
    [SerializeField] private PoolEntry[] pools;

    private Dictionary<GameObject, MirrorPrefabPool> poolMap = new Dictionary<GameObject, MirrorPrefabPool>();

    private void Start()
    {
        InitPools();
    }
    public void InitPools()
    {
        poolMap.Clear();
        foreach (var entry in pools)
        {
            if (entry.prefab == null) continue;

            if (poolMap.ContainsKey(entry.prefab)) continue;

            var pool = new MirrorPrefabPool(entry.prefab, entry.capacity);
            pool.Warnup(entry.warmup);

            poolMap.Add(entry.prefab, pool);
        }
    }

    public MirrorPrefabPool GetPool(GameObject prefab)
    {
        return poolMap[prefab];
    }
}
