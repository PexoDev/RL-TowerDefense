using System;
using Assets.Scripts;
using Assets.Scripts.Units;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitData Data
    {
        get => _data;
        set
        {
            _data = Instantiate(value);
            InitializeUnitData();
        }
    }

    [SerializeField] private UnitData _data;
    private SpriteRenderer spriteRenderer;
    private Transform target;
    private int waypointIndex = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        spriteRenderer.sortingOrder = 1;
        target = GameController.Instance.Path[0].transform;
        InitializeUnitData();
    }

    private void OnEnable()
    {
        TimeController.Instance.OnNextFrame += MoveUnit;
    }

    private void OnDisable()
    {
        TimeController.Instance.OnNextFrame -= MoveUnit;
    }

    private void InitializeUnitData()
    {
        if (_data == null) return;
        if (Data.Sprites.Length > 0)
            spriteRenderer.sprite = Data.Sprites[0];
    }

    private void MoveUnit(FramesUpdate framesUpdate)
    {
        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * Data.SpeedPerFrame * framesUpdate.FrameCount * 0.001f, Space.World);

        if (Vector3.Distance(transform.position, target.position) <= 0.4f)
        {
            GetNextWaypoint();
        }

        Animate();
    }

    public void TakeDamage(int amount, Element element)
    {
        amount = DamageHelper.CalculateElementalDamage(amount, element, Data.Element);

        Data.Health -= amount;
        if (Data.Health <= 0)
        {
            Die();
        }
    }

    private void GetNextWaypoint()
    {
        if (waypointIndex >= GameController.Instance.Path.Length - 1)
        {
            EndPath();
            return;
        }
        waypointIndex++;
        target = GameController.Instance.Path[waypointIndex].transform;
    }

    private void EndPath()
    {
        GameController.Instance.Castle.Damage(Data.Health);
        Destroy(gameObject);
    }

    public void Die()
    {
        OnDeath(this);
        Destroy(gameObject);
    }

    private int spriteIndex = 0;
    private int framesPerSprite = 30;
    private int currentAnimationFrame = 0;
    public void Animate()
    {
        currentAnimationFrame++;
        if(currentAnimationFrame >= framesPerSprite)
        {
            currentAnimationFrame = 0;
            spriteIndex = (spriteIndex + 1) % Data.Sprites.Length;
            spriteRenderer.sprite = Data.Sprites[spriteIndex];
        }
    }

    public event Action<Unit> OnDeath = unit => {};
}
