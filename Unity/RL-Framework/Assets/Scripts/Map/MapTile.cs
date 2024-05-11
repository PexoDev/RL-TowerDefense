using Assets.Scripts.Map;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    private SpriteRenderer _renderer;
    public TileType Type { get; set; } = TileType.Empty;
    public Tower Tower { get; set; }

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(Sprite sprite)
    {
        _renderer.sprite = sprite;
    }

    public void FillTileEncoding(ref int[] tileEncoding)
    {
        tileEncoding[(int)Type] = 1;
        if (Tower != null)
        {
            int towerIndex = (int)Tower.Data.Type * 3 + (int)Tower.Data.Element;
            tileEncoding[4 + towerIndex] = 1;
        }
    }
}