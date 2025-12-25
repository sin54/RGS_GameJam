using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ResourceManager resourceManager;
    public GridRenderer gridRenderer;
    public MainTree mainTree;
    public TowerManager towerManager;
    public SpawnManager spawnManager;
    public PoolManager poolManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
