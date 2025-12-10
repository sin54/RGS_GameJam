using UnityEngine;
using Mirror;

public class PingMarker : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnTypeChanged))]
    public PingType Type;

    [Header("Renderer")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    [SerializeField] private Sprite defendSprite;   // 위 (DefendMf)
    [SerializeField] private Sprite onMyWaySprite;  // 오른쪽 (OnMyWay)
    [SerializeField] private Sprite assistSprite;   // 아래 (AssistMe)
    [SerializeField] private Sprite missingSprite;  // 왼쪽 (Missing)

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 3f;

    private void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        // 서버에서만 수명 타이머 돌리고, 끝나면 네트워크 오브젝트 파괴
        Invoke(nameof(DespawnSelf), lifetime);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        UpdateSprite(Type);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    private void OnTypeChanged(PingType oldType, PingType newType)
    {
        UpdateSprite(newType);
    }

    private void UpdateSprite(PingType type)
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer == null) return;

        switch (type)
        {
            case PingType.DefendMf:
                spriteRenderer.sprite = defendSprite;
                break;
            case PingType.OnMyWay:
                spriteRenderer.sprite = onMyWaySprite;
                break;
            case PingType.AssistMe:
                spriteRenderer.sprite = assistSprite;
                break;
            case PingType.Missing:
                spriteRenderer.sprite = missingSprite;
                break;
        }
    }

    [Server]
    private void DespawnSelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
