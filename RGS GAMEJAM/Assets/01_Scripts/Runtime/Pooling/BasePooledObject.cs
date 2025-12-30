using JamesFrowen.Spawning;
using Mirror;
using UnityEngine;

public class BasePooledObject : PrefabPoolBehaviour
{
    protected virtual void OnSpawned() { }
    protected virtual void OnDespawned() { }

    internal override void BeforeSpawn()
    {
        base.BeforeSpawn();
        OnSpawned();
    }

    internal override void AfterUnspawn()
    {
        OnDespawned();
        base.AfterUnspawn();
    }
    [Server]
    public void ServerDespawn()
    {
        NetworkServer.UnSpawn(gameObject);
    }
}
