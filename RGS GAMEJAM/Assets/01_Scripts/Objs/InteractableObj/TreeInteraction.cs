using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TreeInteraction : NetworkBehaviour, IInteractable
{
    [Header("UI Prompt")]
    [SerializeField] private Transform appearTransform;
    [SerializeField] private bool useAppearTransform = true;

    [Header("TreeData")]
    [SerializeField] private GameObject treePanel;
    [SerializeField] private Image hpImg;
    [SerializeField] private Image xpImg;
    [SerializeField] private TMP_Text hpTxt;
    [SerializeField] private TMP_Text xpTxt;
    [SerializeField] private TMP_Text leafTxt;
    [SerializeField] private TMP_Text stickTxt;
    [SerializeField] private TMP_Text atkTxt;
    [SerializeField] private TMP_Text regenTxt;
    [SerializeField] private TMP_Text levelTxt;

    [SerializeField] private float treeInteractionTime;

    [SerializeField] private SO_Tree treeData;
    [SerializeField] private MainTree mainTree;

    public GameObject Obj => gameObject;
    [SyncVar] public bool isLocked;

    public bool IsLocked { get => isLocked; set => isLocked = value; }
    [SyncVar] private NetworkIdentity lockedBy;
    public NetworkIdentity LockedBy { get => lockedBy; set => lockedBy = value; }
    public Transform AppearTransform => useAppearTransform && appearTransform != null ? appearTransform : transform;
    public bool isAppearTransform => useAppearTransform && appearTransform != null;
    public bool isRoomInteractor => false;
    public InteractionType GetInteractionType() => InteractionType.Hold;
    public float GetHoldTime() => treeInteractionTime;
    public string GetPromptText() => "나뭇가지 얻기";

    public bool CanInteract(Interactor interactor)
    {
        NetworkIdentity playerId;
        if (isServer)
            playerId = interactor.connectionToClient?.identity;
        else
            playerId = interactor.netIdentity;
        if (IsLocked && LockedBy != playerId)
            return false;
        if (NetworkTime.time < mainTree.GetLastInteractionTime() + mainTree.GetInteractionCoolDown())
            return false;

        return true;
    }

    public bool InteractClient(Interactor interactor)
    {
        CustomNetworkGamePlayer.localPlayer.CmdTryGetStick();
        return true;
    }

    public void OnEnterRange(Interactor interactor)
    {
        if (interactor.gameObject.GetComponent<NetworkRoomPlayer>() != null) return;

        if (treePanel != null)
        {
            int level = GameManager.Instance.mainTree.TreeLevel;
            levelTxt.text = "LV" + level.ToString() + " TREE";
            hpTxt.text = GameManager.Instance.mainTree.NowHealth.ToString() + "/" + treeData.maxHps[level].ToString();
            xpTxt.text = GameManager.Instance.mainTree.NowXp.ToString() + "/" + treeData.needXps[level].ToString();
            leafTxt.text = treeData.leafAmount[level] + "EA/" + treeData.leafProduceCool[level].ToString() + "s";
            stickTxt.text = treeData.stickAmount[level] + "EA/" + treeData.stickProduceCool[level].ToString() + "s";
            atkTxt.text = treeData.atkPowers[level].ToString();
            regenTxt.text = treeData.regenAmount[level].ToString() + "/s";

            hpImg.fillAmount = (float)GameManager.Instance.mainTree.NowHealth / treeData.maxHps[level];
            xpImg.fillAmount = (float)GameManager.Instance.mainTree.NowXp / treeData.needXps[level];

            treePanel.SetActive(true);
        }
    }

    public void OnExitRange(Interactor interactor)
    {
        if (treePanel != null) {
            treePanel.SetActive(false);
        }
    }
    [Server]
    public bool InteractServer(Interactor interactor)
    {
        return true;
    }
}
