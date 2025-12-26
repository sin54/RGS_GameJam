using UnityEngine;

[CreateAssetMenu(fileName = "SO_PoolableDB", menuName = "Scriptable Objects/SO_PoolableDB")]
public class SO_PoolableDB : ScriptableObject
{
    public PoolablePrefabEntry[] poolables;

    public GameObject GetPrefab(System.Type type)
    {
        for (int i = 0; i < poolables.Length; i++)
        {
            if (poolables[i].pooledType == type)
                return poolables[i].prefab;
        }
        Debug.LogError($"No prefab for {type}");
        return null;
    }
}
[System.Serializable]
public class PoolablePrefabEntry
{
    public GameObject prefab;
    public System.Type pooledType;
}
