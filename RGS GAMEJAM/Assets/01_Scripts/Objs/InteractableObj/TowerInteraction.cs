using UnityEngine;
using Mirror;
using Unity.Services.Lobbies.Models;
using System.Resources;
public class TowerInteraction : NetworkBehaviour, IInteractable
{
    [Header("UI Prompt")]
    [SerializeField] private Transform appearTransform;
    [SerializeField] private bool useAppearTransform = true;
    public GameObject Obj => gameObject;
    [SyncVar] public bool isLocked;
    public bool IsLocked { get => isLocked; set => isLocked = value; }
    [SyncVar] private NetworkIdentity lockedBy;
    public NetworkIdentity LockedBy { get => lockedBy; set => lockedBy = value; }
    public Transform AppearTransform => useAppearTransform && appearTransform != null ? appearTransform : transform;
    public bool isAppearTransform => useAppearTransform && appearTransform != null;
    public bool isRoomInteractor => false;
    public InteractionType GetInteractionType() => InteractionType.Hold;
    public float GetHoldTime() => BT.towerData.placeTime;
    public string GetPromptText() => "타워 업그레이드";

    private BaseTower BT;
    [SerializeField] private TowerInteractionUI towerUI;
    [SerializeField] private GameObject upgradePanel;
    private void Awake()
    {
        BT = GetComponent<BaseTower>();
    }
    public bool CanInteract(Interactor interactor)
    {
        if (IsLocked && lockedBy != interactor.netIdentity) return false;
        if (BT.towerLevel >= 4) return false;
        if (CanUpgradeResources())
        {
            return true;
        }
        return false;
    }

    public bool InteractClient(Interactor interactor)
    {
        return true;
    }

    public void OnEnterRange(Interactor interactor)
    {
        towerUI.SetTower(BT);
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
        }
        if (BT.towerAttack != null)
        {
            BT.towerAttack.SetRange(true);
        }
    }

    public void OnExitRange(Interactor interactor)
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
        if (BT.towerAttack != null)
        {
            BT.towerAttack.SetRange(false);
        }
    }

    [Server]
    public bool InteractServer(Interactor interactor)
    {
        if (!CanUpgradeResources())
        {
            return false;
        }
        ResourceManager resourceManager = GameManager.Instance.resourceManager;
        resourceManager.ServerRemoveLeaf(BT.towerData.upgradeStuffs[BT.towerLevel+1].needLeaf);
        resourceManager.ServerRemoveStick(BT.towerData.upgradeStuffs[BT.towerLevel+1].needStick);
        resourceManager.ServerRemoveStone(BT.towerData.upgradeStuffs[BT.towerLevel+1].needStone);

        BT.AddTowerLevel();
        return true;
    }
    public void SetTower()
    {
        towerUI.SetTower(BT);
    }
    private bool CanUpgradeResources()
    {
        ResourceManager resourceManager = GameManager.Instance.resourceManager;
        return resourceManager.leaf >= BT.towerData.upgradeStuffs[BT.towerLevel+1].needLeaf &&
            resourceManager.stick >= BT.towerData.upgradeStuffs[BT.towerLevel + 1].needStick &&
            resourceManager.stone >= BT.towerData.upgradeStuffs[BT.towerLevel + 1].needStone;
    }
}
