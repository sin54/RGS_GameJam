using UnityEngine;
using Mirror;
using TMPro;
public class CustomNetworkGamePlayer : NetworkBehaviour
{
    public static CustomNetworkGamePlayer localPlayer;

    [SerializeField] private RuntimeAnimatorController[] characterACs;
    [SerializeField] private TMP_Text playerNameTxt;

    [SerializeField] private Color[] playerNameColorList = new Color[4];

    [SyncVar(hook =nameof(OnIndexChanged))] private int index = 0;
    [SyncVar(hook = nameof(OnCharacterChanged))] private Character currentCharacter;

    [SyncVar(hook = nameof(OnStickChanged))] private int localStick = 0;
    [SyncVar(hook = nameof(OnStoneChanged))] private int localStone = 0;

    private Animator playerAnimator;
    [HideInInspector] public PlayerTower playerTower;
    [HideInInspector] public PlayerMovement playerMovement;

    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerTower = GetComponent<PlayerTower>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        ChangeCharacterAC((int)currentCharacter);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        CharacterChange(((CustomNetworkRoomManager)NetworkManager.singleton).GetCharacter());
        CmdSetIndex(((CustomNetworkRoomManager)NetworkManager.singleton).GetIndex());
        localPlayer = this;
    }

    private void OnCharacterChanged(Character oldValue, Character newValue)
    {
        currentCharacter = newValue;

        if (isLocalPlayer) return;
        ChangeCharacterAC((int)newValue);
    }

    public void CharacterChange(Character character)
    {
        if (!isLocalPlayer) return;

        ChangeCharacterAC((int)character);
        CmdSendCharacter(character);
    }

    [Command]
    private void CmdSendCharacter(Character character)
    {
        currentCharacter = character;
    }

    private void ChangeCharacterAC(int index)
    {
        if (characterACs[index] != null)
        {
            playerAnimator.runtimeAnimatorController = characterACs[index];
        }
        else
        {
            Debug.LogError("No AC for that Character");
        }
    }
    [Command]
    public void CmdTryGetStick()
    {
        GameManager.Instance.mainTree.ServerTryGetStick(this);
    }
    public void AddLocalStick(int amount)
    {
        localStick += amount;
    }
    public void RemoveLocalStick(int amount)
    {
        localStick -= amount;
    }
    public void AddLocalStone(int amount)
    {
        localStone += amount;
    }
    public void RemoveLocalStone(int amount)
    {
        localStone -= amount;
    }
    private void OnStickChanged(int oldValue, int newValue)
    {
        if (!isLocalPlayer) return;

        GameManager.Instance.resourceManager.localStickAmountTxt.text = $"({newValue})";
    }

    private void OnStoneChanged(int oldValue, int newValue)
    {
        if (!isLocalPlayer) return;

        GameManager.Instance.resourceManager.localStoneAmountTxt.text = $"({newValue})";
    }
    public bool isResourceMoved()
    {
        if (localStick > 0) 
        {
            return true;
        }
        if (localStick > 0)
        {
            return true;
        }
        return false;
    }
    [Command]
    public void CmdMoveResourceToGlobal()
    {
        ServerMoveResourceToGlobal();
    }
    [Server]
    private void ServerMoveResourceToGlobal()
    {
        if (localStick > 0)
        {
            localStick--;
            GameManager.Instance.resourceManager.ServerAddStick(1);
        }

        if (localStone > 0)
        {
            localStone--;
            GameManager.Instance.resourceManager.ServerAddStone(1);
        }
    }
    [Command]
    public void CmdRemoveResource()
    {
        ServerRemoveResource();
    }
    [Server]
    private void ServerRemoveResource()
    {
        if(GameManager.Instance.resourceManager.leaf > 0)
        {
            GameManager.Instance.resourceManager.ServerRemoveLeaf(1);
        }
    }

    [Command]
    private void CmdSetIndex(int idx)
    {
        ServerSetIndex(idx);
    }
    [Server]
    private void ServerSetIndex(int idx)
    {
        index = idx;
    }
    private void OnIndexChanged(int oldValue, int newValue)
    {
        playerNameTxt.text = "P" + newValue.ToString();
        playerNameTxt.color = playerNameColorList[newValue-1];
    }
}
