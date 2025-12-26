using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSizeX = 2;
    public int gridSizeY = 2;
    public float cellSize = 0.5f;
    public Color gridColor = Color.white;
    public float lineWidth = 0.02f;
    public Vector3 gridOffset = Vector3.zero;

    [Header("Control")]
    public bool gridEnabled = true;

    [Header("Renderer Settings")]
    public string sortingLayerName = "UI";
    public int sortingOrder = -1;

    [Header("Highlight")]
    public GameObject highlightPrefab;
    [HideInInspector] public GameObject highlightInstance;

    private GameObject gridParent;



    void Start()
    {
        GenerateGrid();
        ApplyGridState();
        ApplyOffset();
        CreateHighlightCell();
    }

    void Update()
    {
        ApplyGridState();
        ApplyOffset();
    }
    private void CreateHighlightCell()
    {
        highlightInstance = Instantiate(highlightPrefab, transform);
        highlightInstance.SetActive(false);
    }
    private void ApplyGridState()
    {
        if (gridParent != null)
            gridParent.SetActive(gridEnabled);
    }

    private void ApplyOffset()
    {
        if (gridParent != null)
            gridParent.transform.localPosition = gridOffset;
    }

    private void GenerateGrid()
    {
        gridParent = new GameObject("IsometricGrid");
        gridParent.transform.parent = transform;
        gridParent.transform.localPosition = gridOffset;

        for (int i = 0; i <= gridSizeX; i++)
        {
            Vector3 start = ToIsometric(new Vector3(i * cellSize, 0, 0));
            Vector3 end = ToIsometric(new Vector3(i * cellSize, 0, gridSizeY * cellSize));
            CreateLine(start, end);
        }

        for (int j = 0; j <= gridSizeY; j++)
        {
            Vector3 start = ToIsometric(new Vector3(0, 0, j * cellSize));
            Vector3 end = ToIsometric(new Vector3(gridSizeX * cellSize, 0, j * cellSize));
            CreateLine(start, end);
        }
    }

    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.parent = gridParent.transform;
        lineObj.transform.localPosition = Vector3.zero;

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = gridColor;
        lr.endColor = gridColor;
        lr.useWorldSpace = false;

        lr.sortingLayerName = sortingLayerName;
        lr.sortingOrder = sortingOrder;
    }

    private Vector3 ToIsometric(Vector3 point)
    {
        float isoX = point.x - point.z;
        float isoY = (point.x + point.z) * 0.5f;
        return new Vector3(isoX, isoY, 0f);
    }

    public void ToggleGrid(bool enabled)
    {
        gridEnabled = enabled;
        ApplyGridState();
    }

    public void SetGridOffset(Vector3 offset)
    {
        gridOffset = offset;
        ApplyOffset();
    }

    public Vector2 ToIsometric2D(Vector2 pos)
    {
        float isoX = pos.x + 2 * pos.y;
        float isoY = 2 * pos.y - pos.x;
        return new Vector2(isoX, isoY);
    }


    public Vector2 ToCartesian2D(float isoX, float isoY)
    {
        float cartX = (isoX - isoY) / 2f;
        float cartY = (isoX + isoY) / 4f;
        return new Vector2(cartX, cartY);
    }

    public Vector2Int HighlightCell(Vector2 cartPos)
    {
        Vector2 isoPos = ToIsometric2D(cartPos);

        float nearestIsoX = Mathf.CeilToInt(isoPos.x)+0.5f;
        float nearestIsoY = Mathf.CeilToInt(isoPos.y)+0.5f;

        Vector2 snappedCart = ToCartesian2D(nearestIsoX, nearestIsoY);

        highlightInstance.transform.localPosition = new Vector2(snappedCart.x, snappedCart.y-0.5f);

        highlightInstance.SetActive(true);
        return new Vector2Int(Mathf.CeilToInt(isoPos.x), Mathf.CeilToInt(isoPos.y));
    }
    public void UnHighlightCell()
    {
        highlightInstance.SetActive(false);
    }
    public Vector2Int CalcHighlightCell(Vector2 cartPos)
    {
        Vector2 isoPos = ToIsometric2D(cartPos);

        float nearestIsoX = Mathf.CeilToInt(isoPos.x) + 0.5f;
        float nearestIsoY = Mathf.CeilToInt(isoPos.y) + 0.5f;

        Vector2 snappedCart = ToCartesian2D(nearestIsoX, nearestIsoY);
        return new Vector2Int(Mathf.CeilToInt(isoPos.x), Mathf.CeilToInt(isoPos.y));
    }
}
