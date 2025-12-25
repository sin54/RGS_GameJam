using UnityEngine;
using Mirror;
public class BasePooledObject : NetworkBehaviour
{
    [HideInInspector] public GameObject originalPrefab;

    public virtual void OnSpawnFromPool()
    {
        gameObject.SetActive(true);
    }
    public virtual void OnDespawnFromPool()
    {

    }

    [Server]
    public void ServerDespawn()
    {
        RpcReturnToPool();
        NetworkServer.UnSpawn(gameObject);
    }

    [ClientRpc]
    private void RpcReturnToPool()
    {
        OnDespawnFromPool();
        GameManager.Instance.poolManager.Return(originalPrefab, gameObject);
    }
}
