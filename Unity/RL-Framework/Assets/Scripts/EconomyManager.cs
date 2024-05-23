using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class EconomyManager : MonoBehaviour
    {
        private int _gold = 0;

        public int Gold
        {
            get => _gold;
            set
            {
                _gold = Mathf.Clamp(value, 0, MaxGold);
                OnGoldUpdate(_gold);
            }
        }

        public event Action<int> OnGoldUpdate = (gold) => { };
        public event Action<int> OnWorkersUpdate = (workers) => { };

        public const int MaxGold = 1000;
        public const int MaxWorkers = 25;
        public const int WorkerCost = 50;
        public const int IncomePerWorker = 1;

        public int NumberOfWorkers { get; private set; }
        private int _framesSinceLastUpdate = 0;

        private void OnEnable()
        {
            TimeController.Instance.OnNextFrame += ProcessFrame;
        }

        private void OnDisable()
        {
            TimeController.Instance.OnNextFrame -= ProcessFrame;
        }

        private void ProcessFrame(FramesUpdate obj)
        {
            _framesSinceLastUpdate += obj.FrameCount;
            if (_framesSinceLastUpdate >= 60)
            {
                _framesSinceLastUpdate = 0;
                Gold += NumberOfWorkers * IncomePerWorker;
            }
        }

        public void BuyWorker()
        {
            if (Gold >= WorkerCost && NumberOfWorkers < MaxWorkers)
            {
                Gold -= WorkerCost;
                NumberOfWorkers++;
                OnWorkersUpdate(NumberOfWorkers);
            }
        }

        public void Reset()
        {
            Gold = 100;
            NumberOfWorkers = 0;
            _framesSinceLastUpdate = 0;
            OnWorkersUpdate(NumberOfWorkers);
        }
    }
}