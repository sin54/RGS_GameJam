using UnityEngine;
using Mirror;
using System.Collections;
public class SpawnManager : NetworkBehaviour
{
    public GameObject[] enemies;
    public GameObject[] towers;
    public GameObject[] bullets;
    public GameObject dmgIndicator;
    public GameObject towerPlacePrefab;
    [SerializeField] private PoolManager poolManager;
    private void Awake()
    {

    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(DelayedSpawn());
    }
    #region SpawnDmgIndicator
    [Server]
    public void SpawnDmgIndicator(Vector2 position, int damage)
    {
        PooledDamageIndicator PDI = poolManager.Spawn<PooledDamageIndicator>(dmgIndicator, position);
        PDI.SetIndicator(damage);
    }
    #endregion
    #region SpawnInteractor
    [Server]
    private void SpawnTowerPlaceForPlayers()
    {
        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn.identity == null) continue;

            PlayerTower playerTower = conn.identity.GetComponent<PlayerTower>();
            if (playerTower == null) continue;

            PooledTowerBatcher PTB = poolManager.Spawn<PooledTowerBatcher>(towerPlacePrefab, playerTower.transform.position);

            PlayerFollowing pf = PTB.GetComponent<PlayerFollowing>();
            if (pf != null)
            {
                pf.SetTarget(playerTower.transform);
            }

            TargetSetupTowerTrigger(conn, PTB.gameObject, playerTower.netIdentity);
        }
    }

    [TargetRpc]
    private void TargetSetupTowerTrigger(NetworkConnection target, GameObject obj, NetworkIdentity playerTowerIdentity)
    {
        PlayerTower myTower = playerTowerIdentity.GetComponent<PlayerTower>();
        PlayerFollowing pf = obj.GetComponent<PlayerFollowing>();
        pf.SetTarget(myTower.transform);

        // PlayerTower 내부에서 참조 저장
        myTower.SetTowerTrigger(obj);
    }
    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnTowerPlaceForPlayers();
    }
    #endregion
    #region SpawnTower
    [Server]
    public GameObject SpawnTower(TowerType type, Vector2 position, Vector2Int isoPos)
    {
        PooledTower PT = poolManager.Spawn<PooledTower>(towers[(int)type-1], position);
        PT.GetComponent<BaseTower>().SetTowerPos(isoPos);
        return PT.gameObject;
    }
    #endregion
    #region SpawnBullet
    public void SpawnBullet(int id, Vector2 position, float damage, Transform target)
    {
        PooledBullet PB = poolManager.Spawn<PooledBullet>(bullets[id], position);
        PB.GetComponent<TowerBullet>().SetBullet(target, damage);
    }
    #endregion
    #region SpawnEnemy
    public void SpawnEnemy(EnemyType enemyType, Vector2 position)
    {
        PooledEnemy PE = poolManager.Spawn<PooledEnemy>(enemies[(int)enemyType], position);
    }
    #endregion
}
