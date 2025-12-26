using UnityEngine;
using Mirror;
public class PlayerFollowing : NetworkBehaviour
{
    private Transform targetPlayer;
    [SerializeField] private TowerPlaceInteraction interaction;
    public void SetTarget(Transform targetPlayer)
    {
        this.targetPlayer = targetPlayer;
        interaction.SetPlayerTower(targetPlayer.GetComponent<PlayerTower>());
    }
    private void Update()
    {
        if (targetPlayer == null)
        {
            gameObject.SetActive(false);
        }
        else
        {
            transform.position = targetPlayer.position;
        }
    }
}
