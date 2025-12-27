using UnityEngine;

public class BaseAttackTower : BaseTower
{
    public SO_BaseAttackTower atkTowerData;

    protected override void Start()
    {
        base.Start();

        atkTowerData = towerData as SO_BaseAttackTower;

        if (atkTowerData == null)
        {
            Debug.LogError("Downcasting Error");
            return;
        }
    }


}
