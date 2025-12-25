using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerInteractionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text towerNameTxt;
    [SerializeField] private TMP_Text towerHPTxt;
    [SerializeField] private TMP_Text towerStuffTxt;

    public void SetTower(BaseTower BT)
    {
        towerNameTxt.text = "LV" + (BT.towerLevel+1).ToString() + " " + BT.towerData.towerName;
        towerHPTxt.text = BT.towerHealth.currentHP.ToString() + "/" + BT.towerData.towerMaxHP[BT.towerLevel].ToString();
        if (BT.towerLevel <=3)
        {
            towerStuffTxt.text = $"<color=green>{BT.towerData.upgradeStuffs[BT.towerLevel+1].needLeaf}</color>/<color=#925a02>{BT.towerData.upgradeStuffs[BT.towerLevel+1].needStick}</color>/<color=#5b5b5b>{BT.towerData.upgradeStuffs[BT.towerLevel+1].needStone}</color>";
        }
        else
        {
            towerStuffTxt.text = "MAX LEVEL";
        }
    }
}
