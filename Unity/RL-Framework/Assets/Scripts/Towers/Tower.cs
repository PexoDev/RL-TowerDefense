using Assets.Scripts.Units;
using System;
using UnityEngine;

namespace Assets.Scripts.Towers
{
    public class Tower : MonoBehaviour
    {
        public GameObject ProjectilePrefab;
        public TowerData Data { get; private set; }
        private int _cooldownTimer;

        private void OnEnable()
        {
            TimeController.Instance.OnNextFrame += ProcessFrame;
        }

        private void OnDisable()
        {
            TimeController.Instance.OnNextFrame -= ProcessFrame;
        }

        public void Initialize(TowerData data)
        {
            Data = Instantiate(data);
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = Data.Sprite;
            renderer.sortingOrder = 2;
            _cooldownTimer = 0;
        }

        private void ProcessFrame(FramesUpdate framesUpdate)
        {
            _cooldownTimer -= framesUpdate.FrameCount;
            if (_cooldownTimer <= 0)
                PerformAttack();
        }

        private void PerformAttack()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, Data.Range);
            foreach (var hitCollider in hitColliders)
            {
                Unit enemy = hitCollider.GetComponent<Unit>();
                if (enemy == null) continue;

                VisualizeProjectile(enemy.transform, () => OnHitEffect(hitCollider));
                _cooldownTimer = Data.CooldownFrames;
                break;
            }
        }

        private void OnHitEffect(Collider2D hitCollider) => Attack(hitCollider.GetComponent<Unit>(), Data.Damage, Data.Element);

        private void Attack(Unit target, int damage, Element element)
        {
            if (target == null) return;
            target.TakeDamage(damage, element);
        }

        private void VisualizeProjectile(Transform target, Action onHit)
        {
            var projectileGO = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
            var projectile = projectileGO.GetComponent<Projectile>();
            projectile.Initialize(target, transform.position);
            projectile.OnHit += onHit;
        }
    }
}