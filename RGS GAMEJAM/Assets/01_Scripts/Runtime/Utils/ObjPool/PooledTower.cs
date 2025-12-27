using UnityEngine;

public class PooledTower : BasePooledObject
{
    public override void OnSpawnFromPool()
    {
        GetComponent<BaseTower>().InitTower();
    }
}
