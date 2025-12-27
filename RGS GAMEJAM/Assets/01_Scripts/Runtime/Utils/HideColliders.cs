using UnityEngine;
using UnityEngine.Tilemaps;
public class HideColliders : MonoBehaviour
{
    private TilemapRenderer tilemapRenderer;

    private void Awake()
    {
        tilemapRenderer = GetComponent<TilemapRenderer>();
    }

    private void Start()
    {
        tilemapRenderer.enabled = false;
    }
}
