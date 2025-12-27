using UnityEngine;

public class EnemyAnimationTrigger : MonoBehaviour
{
    private EnemyCore Core;

    private void Awake()
    {
        Core=GetComponentInParent<EnemyCore>();
    }

    public void OnDeathAnimFinished()
    {
        Core.health.OnDeath();
        Debug.Log("Death");
    }
}
