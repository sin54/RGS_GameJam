using UnityEngine;

public class PlayerChaser : MonoBehaviour
{
    private Transform targetPlayer;

    private void Update()
    {
        if (targetPlayer == null)
        {
            if (CustomNetworkGamePlayer.localPlayer != null)
            {
                targetPlayer = CustomNetworkGamePlayer.localPlayer.transform;
            }
            return;
        }

        transform.position = targetPlayer.position;
    }
}
