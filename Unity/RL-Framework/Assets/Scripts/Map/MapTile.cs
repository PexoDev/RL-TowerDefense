using Assets.Scripts.Towers;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class MapTile : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        public TileType Type { get; set; } = TileType.Empty;
        public Tower Tower { get; set; }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void SetSprite(Sprite sprite)
        {
            _renderer.sprite = sprite;
        }

        public int GetMapEncoding()
        {
            int type = (int)Type;
            int towerIndex = 0;
            if (Tower != null)
            {
                towerIndex = ((int)Tower.Data.Type * 3) + (int)Tower.Data.Element;
            }

            return type + towerIndex;
        }
    }
}