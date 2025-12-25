using UnityEngine;
using Mirror;
using System.Resources;
public class TowerPlaceInteraction : NetworkBehaviour, IInteractable
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
    public float GetHoldTime() => towerData==null?999f:towerData.placeTime;
    public string GetPromptText() => "타워 설치하기";

    private PlayerTower PT;
    private SO_BaseTower towerData;
    [HideInInspector] public bool isMoving;

    public void SetPlayerTower(PlayerTower playerTower)
    {
        this.PT = playerTower;
    }
    public void SetTowerData(SO_BaseTower SO_towerData)
    {
        towerData = SO_towerData;
    }
    public bool CanInteract(Interactor interactor)
    {
        if (!PT.isCellEmpty) return false;
        if (PT.netIdentity != interactor.netIdentity) return false;
        if(isMoving) return false;
        return true;
    }

    public bool InteractClient(Interactor interactor)
    {
        if (PT.isCellEmpty)
        {
            PT.RemoveTower();
            return true;
        }
        else
        {
            return false;
        }
    }

    [Server]
    public bool InteractServer(Interactor interactor)
    {
        return true;
    }

    public void OnEnterRange(Interactor interactor)
    {

    }

    public void OnExitRange(Interactor interactor)
    {
    }

}
