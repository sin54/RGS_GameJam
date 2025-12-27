using UnityEngine;
using Mirror;
public class EnemyCore : NetworkBehaviour
{
    public SO_BaseEnemy enemyData;
    public EnemyType enemyType;

    [Header("Components")]
    [HideInInspector] public EnemyMovement movement;
    [HideInInspector] public EnemyHealth health;
    [HideInInspector] public PooledEnemy pooledEnemy;
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer SR;

    private void Awake()
    {
        movement = GetComponent<EnemyMovement>();
        health = GetComponent<EnemyHealth>();
        pooledEnemy = GetComponent<PooledEnemy>();
        animator = GetComponentInChildren<Animator>();
        SR = GetComponentInChildren<SpriteRenderer>();
    }

}
