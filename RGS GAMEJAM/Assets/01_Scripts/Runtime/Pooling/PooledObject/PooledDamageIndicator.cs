using Mirror;
using TMPro;
using UnityEngine;

public class PooledDamageIndicator : BasePooledObject
{
    [SerializeField] private TMP_Text damageTxt;

    public void ApplyVisual(int damage, float rotation)
    {
        damageTxt.text = damage.ToString();
        transform.localScale = Vector3.one * 0.2f;
        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    protected override void OnDespawned()
    {
        damageTxt.text = "";
    }

    public void OnFinishAnimation()
    {
        if (!NetworkServer.active) return;
        ServerDespawn();
    }
}
