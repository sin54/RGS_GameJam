using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_WaveData", menuName = "Scriptable Objects/SO_WaveData")]
public class SO_WaveData : ScriptableObject
{
    public float waveLength;
    public EnemyData[] randomEnemies;
}
