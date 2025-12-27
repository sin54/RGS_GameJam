using UnityEngine;
[CreateAssetMenu(fileName = "SO_MeleeEnemy", menuName = "Scriptable Objects/Enemy/Melee Enemy")]
public class SO_BaseEnemy : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed;
    public float agroRange;
    public LayerMask towerLayer;
    public LayerMask playerLayer;
    public bool isAgroTower;
    public bool isAgroPlayer;

    [Header("Attack")]
    public float attackRange;
    public float attackCool;
    public float attackDamage;

    [Header("Health")]
    public int maxHealth;
}
