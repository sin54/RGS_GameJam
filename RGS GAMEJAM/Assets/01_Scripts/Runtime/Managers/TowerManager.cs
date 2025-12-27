using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class TowerManager : NetworkBehaviour
{
    [SerializeField] private SO_BaseTower[] towerDataArray;
    public Dictionary<TowerType, SO_BaseTower> towerDataDict = new Dictionary<TowerType, SO_BaseTower>();

    private bool[,] occupied = new bool[48, 48];

    private void Awake()
    {
        foreach (var data in towerDataArray)
        {
            if (data == null) continue;

            if (!towerDataDict.ContainsKey(data.towerType))
            {
                towerDataDict.Add(data.towerType, data);
            }
            else
                Debug.LogWarning($"중복된 타워 타입: {data.towerType}");
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        //workbench
        Place(15, 23);
        Place(15, 24);
        Place(16, 23);
        Place(16, 24);
        //tree
        Place(21, 21);
        Place(21, 22);
        Place(22, 21);
        Place(22, 22);
        //chest
        Place(26, 19);
    }
    [Server]
    public bool CanPlace(int x, int y)
    {
        if (x < 0 || y < 0) return false;

        return occupied[x, y] == false;
    }

    [Server]
    public void Place(int x, int y)
    {
        occupied[x, y] = true;
    }

    [Server]
    public void Remove(int x, int y)
    {
        occupied[x, y] = false;
    }
    [Server] 
    public void PlaceTower(int x, int y, TowerType type)
    {
        Place(x+24, y+24);
        Vector2 cartesiancoord = GameManager.Instance.gridRenderer.ToCartesian2D(x+0.5f, y+0.5f);
        GameManager.Instance.spawnManager.SpawnTower(type, new Vector2(cartesiancoord.x, cartesiancoord.y - 0.5f), new Vector2Int(x+24, y+24));
    }
}
