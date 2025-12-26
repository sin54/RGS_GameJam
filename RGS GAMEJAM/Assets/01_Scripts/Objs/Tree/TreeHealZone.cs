using UnityEngine;
using Mirror;
using System.Collections.Generic;
public class TreeHealZone : MonoBehaviour
{
    [SerializeField] private float cool = 1f;

    private HashSet<GameObject> playersInside = new HashSet<GameObject>();
    private Dictionary<GameObject, float> coolTimers = new Dictionary<GameObject, float>();
    [SerializeField] private MainTree MT;

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playersInside.Add(other.gameObject);
            coolTimers[other.gameObject] = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playersInside.Remove(other.gameObject);
            coolTimers.Remove(other.gameObject);
        }
    }


    private void Update()
    {
        if (!NetworkServer.active) return;
        float dt = Time.deltaTime;

        foreach (var player in playersInside)
        {
            coolTimers[player] += dt;

            if (coolTimers[player] >= cool)
            {
                coolTimers[player] = 0f;
                DoEffect(player);
            }
        }
    }

    [Server]
    private void DoEffect(GameObject player)
    {
        if (player != null && player.layer == 6)
        {
            player.gameObject.GetComponent<PlayerHealth>().Heal(MT.treeData.regenAmount[MT.TreeLevel]);
        }

    }
}
