using UnityEngine;

[CreateAssetMenu(fileName = "SO_SpikeVine", menuName = "Scriptable Objects/Tower/SO_SpikeVine")]
public class SO_SpikeVine : SO_BaseTower
{
    public float[] attackRange = new float[5];
    public int[] attackDamage = new int[5];
}
