using System;
using Assets.Scripts.Units;
using UnityEngine;

namespace Assets.Scripts
{
    public class Unit : MonoBehaviour
    {

        public event Action<Unit, bool> OnDeath = (unit, reachedCastle) => { };

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
        private SpriteRenderer _spriteRenderer;
        private Transform _target;
        private int _waypointIndex;
        private bool _swingingLeft;
        private int _framesSwinging;

        private void OnEnable()
        {
            TimeController.Instance.OnNextFrame += MoveUnit;
        }

        private void OnDisable()
        {
            TimeController.Instance.OnNextFrame -= MoveUnit;
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

        public int GetUnitEncoding()
        {
            return ((int)Data.Type * 3) + (int)Data.Element;
        }

        public void Die(bool reachedCastle = false)
        {
            OnDeath(this, reachedCastle);
            Destroy(gameObject);
        }

        private void InitializeUnitData()
        {
            if (GameController == null || _data == null) return;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sortingOrder = 1;
            _target = GameController.Path[0].transform;
            SetSprite();

            float sizeMultiplier = GetSizeMultiplier();
            _spriteRenderer.transform.localScale = 0.1f * sizeMultiplier * Vector3.one;

            if (UnityEngine.Random.Range(0, 2) == 0)
            {
                _spriteRenderer.flipX = true;
                _swingingLeft = true;
            }
        }

        private void SetSprite()
        {
            if (Data.Sprites.Length > 0)
                _spriteRenderer.sprite = Data.Sprites[0];
        }

        private float GetSizeMultiplier()
        {
            return Data.Type switch
            {
                UnitType.Goblin => 0.5f,
                UnitType.Orc => 1f,
                UnitType.Troll => 2f,
                _ => 1f
            };
        }

        private void MoveUnit(FramesUpdate framesUpdate)
        {
            if (_target == null) return;

            Vector3 dir = _target.position - transform.position;
            transform.Translate(Data.SpeedPerFrame * framesUpdate.FrameCount * 0.001f * dir.normalized, Space.World);

            Animate(framesUpdate);

            if (Vector3.Distance(transform.position, _target.position) <= 0.4f)
            {
                GetNextWaypoint();
            }
        }

        private void Animate(FramesUpdate framesUpdate)
        {
            RotateUnit(framesUpdate);

            if (_framesSwinging >= 20)
            {
                _framesSwinging = 0;
                ToggleSwingDirection();
            }

            _framesSwinging += framesUpdate.FrameCount;
        }

        private void RotateUnit(FramesUpdate framesUpdate)
        {
            float rotationAngle = _swingingLeft ? 1f : -1f;
            transform.Rotate(Vector3.forward, rotationAngle * framesUpdate.FrameCount);
        }

        private void ToggleSwingDirection()
        {
            _swingingLeft = !_swingingLeft;
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }

        private void GetNextWaypoint()
        {
            if (_waypointIndex >= GameController.Path.Length - 1)
            {
                EndPath();
                return;
            }
            _waypointIndex++;
            _target = GameController.Path[_waypointIndex].transform;
        }

        private void EndPath()
        {
            GameController.Castle.Damage(Data.Damage);
            Die(true);
        }
    }
}
