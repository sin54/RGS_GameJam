using Mirror;
using UnityEngine;

public class DamageIndicatorNet : NetworkBehaviour
{
    private PooledDamageIndicator indicator;

    private void Awake()
    {
        indicator = GetComponent<PooledDamageIndicator>();
    }

    [Server]
    public void SetIndicator(int damage)
    {
        RpcSetIndicator(damage, Random.Range(-15f, 15f));
    }

    [ClientRpc]
    private void RpcSetIndicator(int damage, float rotation)
    {
        indicator.ApplyVisual(damage, rotation);
    }
}
