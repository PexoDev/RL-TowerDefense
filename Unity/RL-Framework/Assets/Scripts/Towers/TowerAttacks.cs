using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Towers
{
    public static class TowerAttacks
    {
        public const float MageExplosionRange = 2f;

        public static Dictionary<TowerType, Action<Unit, int, Element>> TowerAttacksMap { get; } = new()
        {
            { TowerType.Archer, ArcherAttack },
            { TowerType.Mage, MageAttack },
            { TowerType.Cannon, CannonAttack }
        };

        private static void ArcherAttack(Unit target, int damage, Element element)
        {
            target.TakeDamage(damage, element);
        }

        private static void MageAttack(Unit target, int damage, Element element)
        {
            GetEnemiesWithinExplosionRange(target, MageExplosionRange).ToList().ForEach(enemy => enemy?.TakeDamage(damage, element));
        }

        private static void CannonAttack(Unit target, int damage, Element element)
        {
            target.TakeDamage(damage, element);
        }

        private static IEnumerable<Unit> GetEnemiesWithinExplosionRange(Unit target, float explosionRange)
        {
            Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, explosionRange);
            IEnumerable<Unit> enemies = hitColliders.Select(collider => collider.GetComponent<Unit>())
                .Where(unit => unit != null);
            return enemies;
        }
    }
}