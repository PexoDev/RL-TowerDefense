using System;
using Assets.Scripts.Towers;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Tower : MonoBehaviour
{
    public GameObject ProjectilePrefab;
    public TowerData Data { get; private set; }
    private int cooldownTimer;

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
        cooldownTimer = 0;

        transform.GetChild(0).transform.localScale = Vector3.one * Data.Range * 10;
    }

    private void ProcessFrame(FramesUpdate framesUpdate)
    {
        cooldownTimer -= framesUpdate.FrameCount;
        if (cooldownTimer <= 0)
        {
            PerformAttack();
        }

        if (cooldownTimer <= 0)
        {
            cooldownTimer = 0;
        }
    }

    private void PerformAttack()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, Data.Range);
        foreach (var hitCollider in hitColliders)
        {
            Unit enemy = hitCollider.GetComponent<Unit>();
            if (enemy == null) continue;

            //VisualizeProjectile(enemy.transform, () => OnHitEffect(hitCollider));
            OnHitEffect(hitCollider);
            cooldownTimer = Data.CooldownFrames;
            break;
        }

        void OnHitEffect(Collider2D hitCollider)
        {
            TowerAttacks.TowerAttacksMap[Data.Type](hitCollider.GetComponent<Unit>(), Data.Damage, Data.Element);
        }

        void VisualizeProjectile(Transform target, Action onHit)
        {
            var projectileGO = Instantiate(ProjectilePrefab, transform.position, Quaternion.identity);
            var projectile = projectileGO.GetComponent<Projectile>();
            projectile.Initialize(target, transform.position);
            projectile.OnHit += onHit;
        }
    }
}