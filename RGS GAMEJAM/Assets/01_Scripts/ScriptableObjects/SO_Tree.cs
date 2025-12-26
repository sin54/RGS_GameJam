using UnityEngine;

[CreateAssetMenu(fileName = "SO_Tree", menuName = "Scriptable Objects/SO_Tree")]
public class SO_Tree : ScriptableObject
{
    public int[] needXps = new int[12];
    public int[] maxHps = new int[12];
    public int[] atkPowers = new int[12];
    public float[] leafProduceCool = new float[12];
    public int[] leafAmount = new int[12];
    public float[] stickProduceCool = new float[12];
    public int[] stickAmount = new int[12];
    public float[] regenAmount = new float[12];
}
