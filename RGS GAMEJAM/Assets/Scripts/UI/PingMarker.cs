using UnityEngine;
using Mirror;

public class PingMarker : NetworkBehaviour
{
    // 타입이 바뀔 때마다 클라에서 스프라이트 갱신되도록 hook 달기
    [SyncVar(hook = nameof(OnTypeChanged))]
    public PingType Type;

    [Header("Renderer")]
    [SerializeField] private SpriteRenderer spriteRenderer;   // 자식 Renderer에 있는 SpriteRenderer

    [Header("Sprites")]
    [SerializeField] private Sprite defendSprite;   // 위 (DefendMf)
    [SerializeField] private Sprite onMyWaySprite;  // 오른쪽 (OnMyWay)
    [SerializeField] private Sprite assistSprite;   // 아래 (AssistMe)
    [SerializeField] private Sprite missingSprite;  // 왼쪽 (Missing)

    [Header("Lifetime")]
    [SerializeField] private float lifetime = 3f;

    private void Awake()
    {
        // 인스펙터에서 안 넣어줬으면 자식에서 자동으로 찾기
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // 서버에서만 수명 타이머 돌리기
    public override void OnStartServer()
    {
        base.OnStartServer();
        Invoke(nameof(DestroySelf), lifetime);
    }

    // 클라이언트에서 처음 스폰될 때 현재 Type 값으로 한 번 초기화
    public override void OnStartClient()
    {
        base.OnStartClient();
        UpdateSprite(Type);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    // SyncVar hook
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
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
