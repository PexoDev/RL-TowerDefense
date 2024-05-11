using System;
using UnityEngine;

namespace Assets.Scripts.Towers
{
    public class Projectile : MonoBehaviour
    {
        public int FramesBeforeHit = 30;
        public event Action OnHit = () => { };

        private Transform _target;
        private int _framesPassed;
        private Vector3 _startingPosition;
        private bool _enabled = false;
        public void Initialize(Transform target, Vector3 start)
        {
            _target = target;
            _startingPosition = start;
            TimeController.Instance.OnNextFrame += MoveTowardsTarget;
            _enabled = true;
        }

        private void MoveTowardsTarget(FramesUpdate framesUpdate)
        {
            if (!_enabled) return;

            if (_target == null)
            {
                _enabled = false;
                Destroy(gameObject);
                return;
            }

            _framesPassed += framesUpdate.FrameCount;
            transform.position = Vector3.Lerp(_startingPosition, _target.position, _framesPassed/(FramesBeforeHit*1f));
            transform.Rotate(Vector3.forward, 1f);    
            if (_framesPassed >= FramesBeforeHit)
            {
                OnHit();
                TimeController.Instance.OnNextFrame -= MoveTowardsTarget;
                _enabled = false;
                Destroy(gameObject);
            }
        }
    }
}