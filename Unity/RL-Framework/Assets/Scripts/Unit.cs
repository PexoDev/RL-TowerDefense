using System;
using Assets.Scripts;
using Assets.Scripts.Units;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GameController _gameController;

    public GameController GameController
    {
        get => _gameController;
        set
        {
            _gameController = value; 
            InitializeUnitData();
        }
    }

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
        if(GameController == null) return;
        if (_data == null) return;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;
        target = GameController.Path[0].transform;
        if (Data.Sprites.Length > 0)
            spriteRenderer.sprite = Data.Sprites[0];
        
        float sizeMultiplier = Data.Type switch
        {
            UnitType.Goblin => 0.5f,
            UnitType.Orc => 1f,
            UnitType.Troll => 2f,
            _ => 1f
        };
        
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            spriteRenderer.flipX = true;
            _swingingLeft = true;
        }

        spriteRenderer.transform.localScale = Vector3.one * 0.5f * sizeMultiplier;
    }

    private bool _swingingLeft;
    private int _framesSwinging = 0;
    private void MoveUnit(FramesUpdate framesUpdate)
    {
        if(target == null) return;

        Vector3 dir = target.position - transform.position;
        transform.Translate(dir.normalized * Data.SpeedPerFrame * framesUpdate.FrameCount * 0.001f, Space.World);

        if (_swingingLeft)
            transform.Rotate(Vector3.forward, 1f * framesUpdate.FrameCount);
        else
            transform.Rotate(Vector3.forward, -1f * framesUpdate.FrameCount);

        if (_framesSwinging >= 20)
        {
            _framesSwinging = 0;
            _swingingLeft = !_swingingLeft;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        _framesSwinging += framesUpdate.FrameCount;

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
        if (waypointIndex >= GameController.Path.Length - 1)
        {
            EndPath();
            return;
        }
        waypointIndex++;
        target = GameController.Path[waypointIndex].transform;
    }

    private void EndPath()
    {
        GameController.Castle.Damage(Data.Damage);
        Die();
    }

    public void GetUnitEncoding(ref int[] encoding)
    {
        encoding[(int)Data.Type] = 1;
        encoding[3 + (int)Data.Element] = 1;
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
