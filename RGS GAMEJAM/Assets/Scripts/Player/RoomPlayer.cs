using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayer : NetworkBehaviour
{
    public static RoomPlayer localPlayer;

    [SyncVar(hook = nameof(OnCharacterChanged))]
    [SerializeField] private Character selectedCharacter;


    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        localPlayer = this;  // LocalPlayer가 Spawn될 때 등록
    }
    public void CharacterChange(Character character)
    {
        selectedCharacter = character;
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
    }
}
