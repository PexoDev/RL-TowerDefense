using UnityEngine;

namespace Assets.Scripts.Units
{
    [CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
    public class UnitData : ScriptableObject
    {
        public Sprite[] Sprites;
        public float SpeedPerFrame;
        public int Health;
        public int Damage;
        public int Cost;
        public int Reward;
        public int BuildTime;
        public UnitType Type;
        public Element Element;

        public void GetUnitEncoding(ref int[] encoding)
        {
            encoding[(int)Type] = 1;
            encoding[3 + (int)Element] = 1;
        }
    }
}