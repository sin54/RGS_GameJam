using UnityEngine;
using Mirror;
public class EnemyCore : NetworkBehaviour
{
    public SO_BaseEnemy enemyData;
    public EnemyType enemyType;

    [Header("Components")]
    [HideInInspector] public EnemyMovement movement;
    [HideInInspector] public EnemyHealth health;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer SR;
    [HideInInspector] public BasePooledObject BPO;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();
        SR = GetComponentInChildren<SpriteRenderer>();
        BPO=GetComponent<BasePooledObject>();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        InitServer();
    }
    [Server]
    private void InitServer()
    {
        health.Init();
    }
}
