using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class EnemyHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))] public int currentHealth;

    [SerializeField] private Image enemyHealthImg;
    [SerializeField] private Transform DmgIndicatorPos;

    [HideInInspector][SyncVar] public bool isDead;
    private EnemyCore Core;

    private void Awake()
    {
        Core = GetComponent<EnemyCore>();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = Core.enemyData.maxHealth;

    }
    [Server]
    public void TakeDamage(int damage)
    {
        RpcHit();
        currentHealth -= damage;
        GameManager.Instance.spawnManager.SpawnDmgIndicator(DmgIndicatorPos.position, damage);
        if (currentHealth <= 0)
        {
            isDead = true;
            Core.movement.StopMove();
            RpcDeath();
        }
    }
    [Server]
    public void OnDeath()
    {
        if (isServer)
        {
            Core.pooledEnemy.ServerDespawn();
        }

    }
    [ClientRpc]
    private void RpcHit()
    {
    }
    [ClientRpc]
    private void RpcDeath()
    {
        gameObject.layer = 11;
        Core.animator.SetTrigger("Dead");
    }

    private void OnHealthChanged(int oldValue, int newValue)
    {
        enemyHealthImg.fillAmount = (float)currentHealth / Core.enemyData.maxHealth;
    }

}
