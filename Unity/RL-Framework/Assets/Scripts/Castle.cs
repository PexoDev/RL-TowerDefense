using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class Castle : MonoBehaviour
    {
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
    }
}