using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class BaseTower : NetworkBehaviour
{
    public SO_BaseTower towerData;
    [SyncVar(hook = nameof(OnTowerLevelChanged))] public int towerLevel;

    [HideInInspector] public TowerInteraction towerInteraction;
    [HideInInspector] public TowerHealth towerHealth;
    [HideInInspector] public TowerAttack towerAttack;
    [HideInInspector] public PooledTower pooledTower;
    [HideInInspector] public Animator animator;
    private Vector2Int towerPos;
    protected virtual void Awake()
    {
        towerInteraction = GetComponent<TowerInteraction>();
        towerHealth = GetComponent<TowerHealth>();
        pooledTower = GetComponent<PooledTower>();
        animator = GetComponentInChildren<Animator>();
        towerAttack = GetComponent<TowerAttack>();
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
    }
    [Server]
    public void InitTower()
    {
        towerLevel = 0;
        animator.SetInteger("Level", towerLevel);
        towerHealth.SetCurrentHP(towerData.towerMaxHP[towerLevel]);
        towerInteraction.SetTower();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

    }
    #region hook

    private void OnTowerLevelChanged(int oldValue, int newValue)
    {
        towerInteraction.SetTower();
    }
    #endregion
    [Server]
    public void AddTowerLevel()
    {
        float towerHPratio = (float)towerHealth.currentHP / towerData.towerMaxHP[towerLevel];
        towerLevel++;
        animator.SetInteger("Level", towerLevel);
        animator.SetTrigger("LevelUp");
        towerHealth.SetCurrentHP(Mathf.CeilToInt(towerHPratio * towerData.towerMaxHP[towerLevel]));
    }
    public void SetTowerPos(Vector2Int pos)
    {
        towerPos = pos;
    }
    public Vector2Int GetTowerPos()
    {
        return towerPos;
    }

}
