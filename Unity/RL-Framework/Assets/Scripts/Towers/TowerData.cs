﻿using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "Tower")]
public class TowerData : ScriptableObject
{
    public Sprite Sprite;
    public float Range;
    public int Damage;
    public int CooldownFrames;
    public int Cost;
    public TowerType Type;
    public Element Element;
}