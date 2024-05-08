using System.Numerics;
using Assets.Scripts.Towers;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using Vector3 = UnityEngine.Vector3;

public class Tower : MonoBehaviour
{
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
        renderer.color = Color.green;
        renderer.sortingOrder = 2;
        cooldownTimer = 0;

        transform.GetChild(0).transform.localScale = Vector3.one * Data.Range * 2;
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
            if (enemy != null) 
            {
                TowerAttacks.TowerAttacksMap[Data.Type](hitCollider.GetComponent<Unit>(), Data.Damage, Data.Element);
                VisualizeHitWithLineRenderer(enemy.transform.position);
                cooldownTimer = Data.CooldownFrames;
                break;
            }
        }
    }

    private void VisualizeHitWithLineRenderer(Vector3 target)
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target);

        Destroy(lineRenderer, 1);
    }
}