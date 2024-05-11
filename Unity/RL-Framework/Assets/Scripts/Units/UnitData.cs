using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class UnitData : ScriptableObject
{
    public Sprite[] Sprites;
    public float SpeedPerFrame;
    public int Health;
    public int Damage;
    public int Cost;
    public int Reward;
    public float BuildTime;
    public UnitType Type;
    public Element Element;

    public void GetUnitEncoding(ref int[] encoding)
    {
        encoding[(int)Type] = 1;
        encoding[3 + (int)Element] = 1;
    }
}