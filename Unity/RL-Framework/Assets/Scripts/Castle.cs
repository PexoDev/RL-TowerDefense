using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Castle : MonoBehaviour
    {
        public event Action<int> OnCastleHit = i => { };
        public GameController GameController { get; set; }

        public int Health => _health;
        private int _health = MaxHealth;
        public const int MaxHealth = 200;

        public void Damage(int amount)
        {
            if (_health <= 0) return;

            _health -= amount;
            OnCastleHit(amount);
            if (_health <= 0)
            {
                GameController.GameOver(false);
            }
        }
    }
}