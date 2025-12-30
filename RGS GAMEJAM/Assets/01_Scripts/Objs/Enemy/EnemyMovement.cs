using UnityEngine;
using Mirror;
public class EnemyMovement : NetworkBehaviour
{
    private Rigidbody2D rb;
    private Transform mainTarget;
    [HideInInspector] public Transform currentTarget;
    private EnemyCore Core;
    private bool canMove;
    [SyncVar(hook = nameof(OnFlipChanged))] private bool flipX;
    private void Awake()
    {
        Core = GetComponent<EnemyCore>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        mainTarget = GameObject.FindGameObjectWithTag("mainTree").transform;
        canMove = true;
        flipX = false;
    }

    private void FixedUpdate()
    {
        if (!isServer) return;
        if (!canMove) return;

        UpdateAgroTarget();

        Vector2 dir;

        if (currentTarget != null)
        {
            dir = (currentTarget.position - transform.position).normalized;
        }
        else
        {
            dir = (mainTarget.position - transform.position).normalized; 
        }

        rb.MovePosition(rb.position + dir * Core.enemyData.moveSpeed * Time.fixedDeltaTime);

        if (dir.x < 0)
        {
            flipX = true;
        }
        else
        {
            flipX = false;
        }
    }

    private void UpdateAgroTarget()
    {
        
        Transform nearest = null;
        float nearestDist = Mathf.Infinity;

        if (Core.enemyData.isAgroTower)
        {
            Collider2D[] col_tower = Physics2D.OverlapCircleAll(transform.position, Core.enemyData.agroRange, Core.enemyData.towerLayer);

            foreach (var col in col_tower)
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = col.transform;
                }
            }
        }

        if (Core.enemyData.isAgroPlayer)
        {
            Collider2D[] col_player = Physics2D.OverlapCircleAll(transform.position, Core.enemyData.agroRange, Core.enemyData.playerLayer);

            foreach (var col in col_player)
            {
                float dist = Vector2.Distance(transform.position, col.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = col.transform;
                }
            }
        }

        currentTarget = nearest;
    }
    public void StopMove()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
    }
    public void ReMove()
    {
        canMove = true;
    }
    private void OnFlipChanged(bool oldValue, bool newValue)
    {
        Core.SR.flipX = newValue;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer) return;
        if (collision.collider.CompareTag("mainTree"))
        {
            GameManager.Instance.mainTree.OnTreeAttacked(Core.health.currentHealth);
            Core.BPO.ServerDespawn();
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, Core.enemyData.agroRange);
    }
}
