using UnityEngine;
using UnityEngine.UI;
using Mirror;
using Unity.Services.Lobbies.Models;
public class TowerHealth : NetworkBehaviour,IEnemyAttackable
{
    [SyncVar(hook = nameof(OnHPAmountChanged))] public int currentHP;

    [SerializeField] private Image towerHPImg;
    private BaseTower BT;

    private void Awake()
    {
        BT = GetComponent<BaseTower>();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHP = BT.towerData.towerMaxHP[0];
    }

    [Server]
    public void TakeDamage(float damage)
    {
        currentHP -= Mathf.CeilToInt(damage);

        if (currentHP <= 0) {
            OnTowerDestroy();
        }
    }

    [Server]
    public void OnTowerDestroy()
    {
        GameManager.Instance.towerManager.Remove(BT.GetTowerPos().x, BT.GetTowerPos().y);
        BT.pooledTower.ServerDespawn();
    }
    [Server]
    public void SetCurrentHP(int amount)
    {
        currentHP = 0;
        currentHP = amount;
    }
    private void OnHPAmountChanged(int oldValue, int newValue)
    {
        towerHPImg.fillAmount = (float)newValue / BT.towerData.towerMaxHP[BT.towerLevel];
        BT.towerInteraction.SetTower();
    }
}
