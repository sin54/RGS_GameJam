using Mirror;
using System;
using TMPro;
using UnityEngine;

public class ResourceManager : NetworkBehaviour
{
    public class BluePrint : SyncDictionary<TowerType, bool> { }

    [SyncVar(hook =nameof(OnChangeLeafAmount))] public int leaf;
    [SyncVar(hook = nameof(OnChangeStickAmount))] public int stick;
    [SyncVar(hook = nameof(OnChangeStoneAmount))] public int stone;
     public readonly BluePrint bluePrints = new BluePrint();

    [SerializeField] private TMP_Text leafAmountTxt;
    [SerializeField] private TMP_Text stickAmountTxt;
    [SerializeField] private TMP_Text stoneAmountTxt;

    public TMP_Text localStickAmountTxt;
    public TMP_Text localStoneAmountTxt;

    public override void OnStartServer()
    {
        base.OnStartServer();
        foreach (TowerType type in Enum.GetValues(typeof(TowerType)))
        {
            if (!bluePrints.ContainsKey(type))
                bluePrints.Add(type, false);
        }
        bluePrints[TowerType.WOOD_CROSSBOW] = true;
        bluePrints[TowerType.WOOD_FENCE] = true;
    }
    public void OnChangeLeafAmount(int oldValue, int newValue)
    {
        leafAmountTxt.text = newValue.ToString();
    }
    public void OnChangeStickAmount(int oldValue, int newValue)
    {
        stickAmountTxt.text = newValue.ToString();
    }
    public void OnChangeStoneAmount(int oldValue, int newValue)
    {
        stoneAmountTxt.text = newValue.ToString();
    }
    #region Server
    [Server]
    public void ServerAddLeaf(int amount)
    {
        leaf += amount;
    }

    [Server]
    public void ServerRemoveLeaf(int amount)
    {
        leaf -= amount;
    }

    [Server]
    public void ServerAddStick(int amount)
    {
        stick += amount;
    }

    [Server]
    public void ServerRemoveStick(int amount)
    {
        stick -= amount;
    }

    [Server]
    public void ServerAddStone(int amount)
    {
        stone += amount;
    }

    [Server]
    public void ServerRemoveStone(int amount)
    {
        stone -= amount;
    }

    [Server]
    public void ServerUnlockBlueprint(TowerType type)
    {
        bluePrints[type] = true;
    }
    #endregion
}
