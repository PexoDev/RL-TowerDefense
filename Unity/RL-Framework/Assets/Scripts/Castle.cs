using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Castle : MonoBehaviour
    {
        public GameController GameController { get; set; }
        public event Action<int> OnCastleHit = i => { }; 
        private int health = 100;
        public void Damage(int amount)
        {
            health -= amount;
            OnCastleHit(amount);
            if (health <= 0)
            {
                GameController.GameOver(false);
            }
        }

        public int Health => health;
        public const int MaxHealth = 100;

        public void ResetCastle()
        {
            health = MaxHealth;
        }
    }
}