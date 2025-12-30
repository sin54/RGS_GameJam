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

    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(DelayedSpawn());
    }

    #region Damage Indicator
    [Server]
    public void SpawnDmgIndicator(Vector2 position, int damage)
    {
        var pool = GameManager.Instance.poolManager.GetPool(dmgIndicator);

        var indicator = pool.Spawn(position, Quaternion.identity);
        NetworkServer.Spawn(indicator.gameObject);

        indicator.GetComponent<DamageIndicatorNet>().SetIndicator(damage);
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

            var batcher = Instantiate(
                towerPlacePrefab,
                playerTower.transform.position,
                Quaternion.identity
            );

            NetworkServer.Spawn(batcher);

            PlayerFollowing pf = batcher.GetComponent<PlayerFollowing>();
            if (pf != null)
                pf.SetTarget(playerTower.transform);

            TargetSetupTowerTrigger(conn, batcher, playerTower.netIdentity);
        }
    }

    [TargetRpc]
    private void TargetSetupTowerTrigger(NetworkConnection target, GameObject obj, NetworkIdentity playerTowerIdentity)
    {
        PlayerTower myTower = playerTowerIdentity.GetComponent<PlayerTower>();
        PlayerFollowing pf = obj.GetComponent<PlayerFollowing>();
        pf.SetTarget(myTower.transform);

        myTower.SetTowerTrigger(obj);
    }
    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnTowerPlaceForPlayers();
    }
    #endregion

    #region Spawn Tower
    [Server]
    public GameObject SpawnTower(TowerType type, Vector2 position, Vector2Int isoPos)
    {
        var prefab = towers[(int)type - 1];
        var pool = GameManager.Instance.poolManager.GetPool(prefab);

        var tower = pool.Spawn(position, Quaternion.identity);
        NetworkServer.Spawn(tower.gameObject);

        tower.GetComponent<BaseTower>().SetTowerPos(isoPos);
        return tower.gameObject;
    }
    #endregion

    #region Spawn Bullet
    [Server]
    public void SpawnBullet(int id, Vector2 position, float damage, Transform target)
    {
        var prefab = bullets[id];
        var pool = GameManager.Instance.poolManager.GetPool(prefab);

        var bullet = pool.Spawn(position, Quaternion.identity);
        NetworkServer.Spawn(bullet.gameObject);

        bullet.GetComponent<TowerBullet>()
              .SetBullet(target, damage);
    }
    #endregion

    #region Spawn Enemy
    [Server]
    public void SpawnEnemy(EnemyType enemyType, Vector2 position)
    {
        var prefab = enemies[(int)enemyType];
        var pool = GameManager.Instance.poolManager.GetPool(prefab);

        var enemy = pool.Spawn(position, Quaternion.identity);
        NetworkServer.Spawn(enemy.gameObject);
    }
    #endregion
}
