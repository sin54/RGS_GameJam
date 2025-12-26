using Mirror;
using UnityEngine;

public class EnemyMeleeAttack : NetworkBehaviour
{
    private EnemyCore Core;
    private float lastAttackTime;
    private Transform target;
    [SerializeField] private LayerMask targetLayerMask;
    private void Awake()
    {
        Core = GetComponent<EnemyCore>();
    }

    private void Update()
    {
        if (!isServer) return;

        target = Core.movement.currentTarget;
        if (target == null) return;
        FindNearestTarget();

        if (Time.time >= lastAttackTime + Core.enemyData.attackCool)
        {
            PerformAttack();
        }
    }

    [Server]
    private void PerformAttack()
    {
        if (target == null) return;

        var attackable = target.GetComponent<IEnemyAttackable>();
        if (attackable != null)
        {
            attackable.TakeDamage(Core.enemyData.attackDamage);
        }

        lastAttackTime = Time.time;
        RpcPlayAttackEffect();
    }

    [Server]
    private void FindNearestTarget()
    {
        target = null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            Core.enemyData.attackRange,
            targetLayerMask
        );

        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                target = hit.transform;
            }
        }
    }

    [ClientRpc]
    private void RpcPlayAttackEffect()
    {
        //TODO: 애니메이션 추가하기
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Core.enemyData.attackRange);
    }
}
