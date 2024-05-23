using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public class EnemySpawner : MonoBehaviour
    {
        public GameController GameController { get; set; }
        public GameObject UnitPrefab { get; set; }
        public Wave[] WavesEnemies { get; set; }
        public bool SpawnPresetWaves { get; set; } = false;
        public const int WaveMaxFrames = 60 * 30;
        public const int Waves = 2;
        private const int FramesBetweenEnemies = 5;

        public event Action<Unit, bool> OnUnitDeath = (unit, reachedCastle) => { };
        public int WaveNumber { get; private set; }
        public readonly Queue<Unit> BuildQueue = new();
        public readonly List<Unit> ActiveUnits = new();

        private bool _readyToSpawn = true;
        private int _framesSinceLastSpawn;
        private int _waveFrames;

        private void OnEnable()
        {
            TimeController.Instance.OnNextFrame += UpdateQueue;
        }

        private void OnDisable()
        {
            TimeController.Instance.OnNextFrame -= UpdateQueue;
        }

        public void QueueEnemy(UnitData unitData)
        {
            var newEnemy = Instantiate(UnitPrefab, transform.position, Quaternion.identity, transform);
            newEnemy.gameObject.SetActive(false);
            var unit = newEnemy.GetComponent<Unit>();
            unit.Data = unitData;
            BuildQueue.Enqueue(unit);
        }

        public void StartWave()
        {
            _waveFrames = 0;

            if (WaveNumber >= Waves)
            {
                GameController.GameOver(true);
                return;
            }

            
            if(SpawnPresetWaves)
                foreach (var unitData in WavesEnemies[WaveNumber].Units())
                    QueueEnemy(unitData);

            WaveNumber++;
            GameController.NextWave(WaveNumber, _waveFrames);
        }

        private void UpdateQueue(FramesUpdate framesUpdate)
        {
            _waveFrames += framesUpdate.FrameCount;
            _framesSinceLastSpawn += framesUpdate.FrameCount;
            if (!_readyToSpawn && _framesSinceLastSpawn >= FramesBetweenEnemies)
            {
                _framesSinceLastSpawn -= FramesBetweenEnemies;
                _readyToSpawn = true;
            }

            if (!_readyToSpawn || BuildQueue.Count <= 0) return;
            if (!(_framesSinceLastSpawn >= BuildQueue.Peek().Data.BuildTime)) return;

            SpawnUnit();

            if (_waveFrames >= WaveMaxFrames)
                StartWave();
        }

        private void SpawnUnit()
        {
            Unit enemyToSpawn = BuildQueue.Dequeue();
            enemyToSpawn.GameController = GameController;
            enemyToSpawn.gameObject.SetActive(true);
            ActiveUnits.Add(enemyToSpawn);

            enemyToSpawn.OnDeath += HandleUnitDeath;

            _framesSinceLastSpawn = 0;
            _readyToSpawn = false;
        }

        private void HandleUnitDeath(Unit u, bool reachedCastle)
        {
            OnUnitDeath(u, reachedCastle);
            ActiveUnits.Remove(u);
            if (ActiveUnits.Count < 1 && BuildQueue.Count < 1)
                StartWave();
        }
    }
}