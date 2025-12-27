using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class PlayerHealth : NetworkBehaviour, IEnemyAttackable
{
    [SerializeField] private int playerMaxHealth;
    [SerializeField] private Image playerHealthImg;
    [SerializeField] private Transform dmgIndicatorPos;
    [SerializeField] private float playerReviveTime;

    private PlayerMovement PM;
    private Animator animator;
    private BoxCollider2D playerCollider;

    [SyncVar(hook = nameof(OnPlayerHealthChanged))] private int playerHealth;
    [SyncVar(hook =nameof(OnDeadChanged))]public bool isDead;

    private void Awake()
    {
        PM = GetComponent<PlayerMovement>();
        animator = GetComponentInChildren<Animator>();
        playerCollider = GetComponent<BoxCollider2D>();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerSetHealth(playerMaxHealth);
    }
    public float GetPlayerReviveTime()
    {
        return playerReviveTime;
    }
    public void TakeDamage(float damage)
    {
        ServerTakeDamage(Mathf.CeilToInt(damage));
    }
    public void Heal(float amount)
    {
        ServerHeal(Mathf.CeilToInt(amount));
    }

    [Server]
    private void ServerTakeDamage(int damage)
    {
        playerHealth -= damage;
        GameManager.Instance.spawnManager.SpawnDmgIndicator(dmgIndicatorPos.position, damage);
        if (playerHealth <= 0)
        {
            isDead = true;
            PM.SetVelocityZero();

        }
    }
    private void OnDeadChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            animator.SetBool("isDead", true);
            gameObject.layer = 7;
            playerCollider.isTrigger = true;
        }
        else
        {
            animator.SetBool("isDead", false);
            gameObject.layer = 6;
            playerCollider.isTrigger = false;
        }
    }
    [Server]
    private void ServerHeal(int amount)
    {
        playerHealth += amount;
        if (playerHealth >= playerMaxHealth)
        {
            playerHealth = playerMaxHealth;
        }
    }
    [Server]
    public void ServerSetHealth(int health) 
    {
        playerHealth = health;
    }
    [Server]
    public void Revive()
    {
        isDead = false;
    }

    private void OnPlayerHealthChanged(int oldValue, int newValue)
    {
        playerHealthImg.fillAmount = (float)newValue / playerMaxHealth;
    }

}
