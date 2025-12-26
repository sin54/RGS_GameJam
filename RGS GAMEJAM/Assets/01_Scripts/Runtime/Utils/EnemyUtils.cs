using UnityEngine;

public enum EnemyType
{
    NormalPerson,
}

[System.Serializable]
public struct EnemyData
{
    public EnemyType enemyType;
    public int count;
}

