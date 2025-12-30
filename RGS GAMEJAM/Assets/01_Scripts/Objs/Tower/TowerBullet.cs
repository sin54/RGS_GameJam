using UnityEngine;
using Mirror;

public class TowerBullet : NetworkBehaviour
{
    [SerializeField] private Sprite bulletSprite;
    [SerializeField] private float speed;

    private float damage;

    private Transform target;
    private Rigidbody2D rb;
    private BasePooledObject BPO;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        BPO = GetComponent<BasePooledObject>();
    }
    public void SetBullet(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }
    private void FixedUpdate()
    {
        if (!isServer) return;

        if (!target.gameObject.activeSelf||target.gameObject.layer!=9)
        {
           
            BPO.ServerDespawn();
            return;
        }

        Vector2 dir = ((Vector2)target.position - rb.position).normalized;

        rb.linearVelocity = dir * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isServer) return;
        if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;
        if (target != null && collision.transform != target)
            return;

        EnemyCore EC = collision.GetComponent<EnemyCore>();
        EC.health.TakeDamage(Mathf.CeilToInt(damage));

        BPO.ServerDespawn();
    }
}
