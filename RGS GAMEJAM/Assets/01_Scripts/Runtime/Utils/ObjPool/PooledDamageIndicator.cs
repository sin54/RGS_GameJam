using TMPro;
using UnityEngine;
using Mirror;
public class PooledDamageIndicator : BasePooledObject
{
    [SerializeField] private TMP_Text damageTxt;

    [Server]
    public void SetIndicator(int damage)
    {
        RpcSetIndicator(damage, Random.Range(-15f,15f));
    }

    [ClientRpc]
    private void RpcSetIndicator(int damage, float rotation)
    {
        damageTxt.text = damage.ToString();
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    public override void OnDespawnFromPool()
    {
        damageTxt.text = "";
    }
    public void OnFinishAnimation() 
    {
        if (isServer)
        {
            ServerDespawn();
        }

    }
}
