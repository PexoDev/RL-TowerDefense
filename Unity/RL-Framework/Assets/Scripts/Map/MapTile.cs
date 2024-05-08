using Assets.Scripts.Map;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    private SpriteRenderer _renderer;
    public TileType Type { get; set; } = TileType.Empty;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite)
    {
        _renderer.sprite = sprite;
    }

    public void OnRaycastHit()
    {
        Debug.Log($"I'm hit! {name}");
    }
}
