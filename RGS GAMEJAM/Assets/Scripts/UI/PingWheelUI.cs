using UnityEngine;
using UnityEngine.UI;

public class PingWheelUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform root;

    [SerializeField] private Image defendImage;
    [SerializeField] private Image onMyWayImage;
    [SerializeField] private Image assistImage;
    [SerializeField] private Image missingImage;

    [SerializeField] private float selectedAlpha = 1f;
    [SerializeField] private float unselectedAlpha = 0.4f;

    private void Awake()
    {
        Close();
    }

    public void Open(Vector2 screenPos)
    {
        gameObject.SetActive(true);

        root.position = screenPos;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        ClearSelection();
    }

    public void Close()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        gameObject.SetActive(false);
    }

    public void SetSelection(PingType type)
    {
        ClearSelection();

        Image target = null;
        switch (type)
        {
            case PingType.DefendMf: target = defendImage; break;   // 위
            case PingType.OnMyWay: target = onMyWayImage; break;  // 오른쪽
            case PingType.AssistMe: target = assistImage; break;   // 아래
            case PingType.Missing: target = missingImage; break;  // 왼쪽
        }

        if (target != null)
        {
            var c = target.color;
            c.a = selectedAlpha;
            target.color = c;
        }
    }

    public void ClearSelection()
    {
        SetAlpha(defendImage, unselectedAlpha);
        SetAlpha(onMyWayImage, unselectedAlpha);
        SetAlpha(assistImage, unselectedAlpha);
        SetAlpha(missingImage, unselectedAlpha);
    }

    private void SetAlpha(Image img, float a)
    {
        if (img == null) return;
        var c = img.color;
        c.a = a;
        img.color = c;
    }
}
