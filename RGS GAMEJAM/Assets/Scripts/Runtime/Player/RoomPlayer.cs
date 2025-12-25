using Mirror;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public class RoomPlayer : NetworkBehaviour
{
    public static RoomPlayer localRoomPlayer;

    [SyncVar(hook = nameof(OnCharacterChanged))]
    [SerializeField] private Character selectedCharacter;
    [SerializeField] private RuntimeAnimatorController[] characterACs;

    private Animator playerAnimator;
    private RoomManager roomManager;
    private void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localRoomPlayer = this;
        roomManager.OnCharacterChanged(Random.Range(0, Enum.GetValues(typeof(Character)).Length));
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        ChangeCharacterAC((int)selectedCharacter);
    }
    public void CharacterChange(Character character)
    {
        selectedCharacter = character;
        ChangeCharacterAC((int)character);
        CmdSendCharacter(character);
    }
    [Command]
    private void CmdSendCharacter(Character character)
    {
        selectedCharacter = character;
    }
    private void OnCharacterChanged(Character oldValue, Character newValue)
    {
        if (isLocalPlayer) return;
        selectedCharacter = newValue;
        ChangeCharacterAC((int)newValue);
    }
    private void ChangeCharacterAC(int newValue)
    {
        if (characterACs[newValue] != null)
        {
            playerAnimator.runtimeAnimatorController = characterACs[newValue];
        }
        else
        {
            Debug.LogError("No AC for that Character");
        }
    }
}
