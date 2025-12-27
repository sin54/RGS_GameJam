using Mirror;
using UnityEngine;

public class TowerAttack : NetworkBehaviour
{
    [SerializeField] private Transform searchPosition;
    [SerializeField] private Transform attackPosition;
    [SerializeField] private GameObject attackRangeObj;
    [SerializeField] private int prefabId;
    private SO_BaseAttackTower attackTowerData;
    private BaseTower BT;

    private float attackTimer = 0f;

    private void Awake()
    {
        BT = GetComponent<BaseTower>();
    }

    private void Start()
    {
        attackTowerData = (SO_BaseAttackTower)BT.towerData;
        attackRangeObj.transform.localScale = Vector3.one * 2 * attackTowerData.attackRange[BT.towerLevel];
        attackRangeObj.SetActive(false);
    }

    private void Update()
    {
        if (!isServer) return;

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            TryAttack();
        }
    }

    [Server]
    private void TryAttack()
    {
        Transform target = GetNearestEnemy();
        if (target == null) return;

        float cool = attackTowerData.attackCool[BT.towerLevel];

        GameManager.Instance.spawnManager.SpawnBullet(prefabId, attackPosition.position,  attackTowerData.attackDamage[BT.towerLevel], target);

        attackTimer = cool;
    }

    [Server]
    private Transform GetNearestEnemy()
    {
        float range = attackTowerData.attackRange[BT.towerLevel];
        float minDist = float.MaxValue;
        Transform nearest = null;
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Collider2D[] hits = Physics2D.OverlapCircleAll(searchPosition.position, range);

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject.layer != enemyLayer)
                continue;

            float dist = Vector2.Distance(searchPosition.position, hit.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearest = hit.transform;
            }
        }

        return nearest;
    }
    public void SetRange(bool isActive)
    {
        attackRangeObj.SetActive(isActive);
    }
    private void OnDrawGizmosSelected()
    {
        if (attackTowerData == null || BT == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackTowerData.attackRange[BT.towerLevel]);
    }
}