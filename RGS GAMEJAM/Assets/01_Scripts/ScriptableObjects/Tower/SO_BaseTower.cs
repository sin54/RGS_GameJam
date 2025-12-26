using UnityEngine;
using UnityEngine.UI;

public class SO_BaseTower : ScriptableObject
{
    public int[] towerMaxHP = new int[5];
    public float placeTime;
    public TowerStuff[] upgradeStuffs=new TowerStuff[5];
    public TowerType towerType;

    public string towerName;
    [TextArea]
    public string towerInfo;
    public Sprite towerIcon;
}
