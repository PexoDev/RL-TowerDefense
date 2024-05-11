using System;
using Assets.Scripts.Players.Attacker;
using Assets.Scripts.Players.Defender;
using Assets.Scripts.Units;
using System.Collections.Generic;
using Assets.Scripts.Map;
using TMPro;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;

#pragma warning disable CS0649 // Assigned via Unity Inspector

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        public event Action OnWaveEnded = () => {};
        public event Action<bool> OnGameOver = defenderWon => { };
        public MapTile[] Path { get; set; }
        public MapTile[,] Map { get; set; }
        public Castle Castle { get; set; }
        private EnemySpawner _spawn;

        public EnemySpawner Spawn
        {
            get => _spawn;
            set
            {
                _spawn = value;
                _spawn.UnitPrefab = _unitPrefab;
                List<Wave> waves = new();
                foreach (Wave w in _waves)
                    waves.Add(Instantiate(w));
                _spawn.WavesEnemies = waves.ToArray();
            }
        }

        [SerializeField] private GameObject _unitPrefab;
        [SerializeField] private UnitData[] _units;
        [SerializeField] private Wave[] _waves;

        [SerializeField] private GameObject _towerPrefab;
        [SerializeField] private TowerData[] _towers;

        public GameObject TowerProjectilePrefab;
        public IDefenderController DefenderController;
        public IAttackerController AttackerController;
        
        public TextMeshProUGUI AttackerGoldText;
        public TextMeshProUGUI AttackerWorkersText;
        public TextMeshProUGUI DefenderGoldText;
        public TextMeshProUGUI DefenderWorkersText;
        public TextMeshProUGUI ResultText;
        private int _attackerWins = 0;
        private int _defenderWins = 0;

        public void GameOver(bool defenderWon)
        {
            OnGameOver(defenderWon);

            if (defenderWon)
                _defenderWins++;
            else
                _attackerWins++;

            ResultText.text = $"{_attackerWins} - {_defenderWins}";
        }


        private void Start()
        {
            DefenderController = new RLDefenderController();
            var defenderEconomyManager = gameObject.AddComponent<EconomyManager>();
            defenderEconomyManager.OnGoldUpdate += gold => DefenderGoldText.text = $"Defender: {gold.ToString()}";
            defenderEconomyManager.OnWorkersUpdate += workers => DefenderWorkersText.text = $"Workers: {workers.ToString()}";
            DefenderController.Initialize(_towers, defenderEconomyManager, _towerPrefab, this);
            var defenderAgent = GetComponentInChildren<DefenderAgent>();
            if (defenderAgent != null)
                defenderAgent.DefenderController = (RLDefenderController)DefenderController;

            AttackerController = new RLAttackerController();
            var attackerEconomyManager = gameObject.AddComponent<EconomyManager>();
            attackerEconomyManager.OnGoldUpdate += gold => AttackerGoldText.text = $"Attacker: {gold.ToString()}";
            attackerEconomyManager.OnWorkersUpdate += workers => AttackerWorkersText.text = $"Workers: {workers.ToString()}";
            AttackerController.Initialize(_units, attackerEconomyManager, Spawn, this);
            var attackerAgent = GetComponentInChildren<AttackerAgent>();
            if (attackerAgent != null)
                attackerAgent.AttackerController = (RLAttackerController)AttackerController;

            Castle.OnCastleHit += dmg => attackerEconomyManager.Gold += dmg;
            _spawn.OnUnitDeath += unit => defenderEconomyManager.Gold += unit.Data.Reward;
            
            _spawn.StartWave();
        }

        public void NextWave(int waveIndex)
        {
            OnWaveEnded();
            DefenderController.EconomyManager.Gold += 100 * waveIndex;
            AttackerController.EconomyManager.Gold += 200 * waveIndex;
        }

        public DefenderObservation GetDefenderObservation()
        {

            var observation = new DefenderObservation()
            {
                MapObservation = GetMapObservation(),
                UnitsObservation = GetUnitsObservation(),
                BuildQueueObservation = BuildQueueObservation(),
                EconomyObservation = GetEconomyObservation(DefenderController.EconomyManager),
                CastleObservation = GetCastleObservation()
            };

            return observation;
        }

        private float[] GetEconomyObservation(EconomyManager economyManager)
        {
            float[] economyObservation = new float[2];
            economyObservation[0] = economyManager.Gold*1f/EconomyManager.MaxGold;
            economyObservation[1] = economyManager.NumberOfWorkers * 1f /EconomyManager.MaxWorkers;
            return economyObservation;
        }

        public AttackerObservation GetAttackerObservation()
        {
            return new()
            {
                MapObservation = GetMapObservation(),
                UnitsObservation = GetUnitsObservation(),
                BuildQueueObservation = BuildQueueObservation(),
                EconomyObservation = GetEconomyObservation(AttackerController.EconomyManager),
                CastleObservation = GetCastleObservation()
            };
        }

        private readonly int[] _mapObservation = new int[MapGenerator.MapSize*MapGenerator.MapSize*13];
        private int[] _tileEncoding = new int[13];
        private int[] GetMapObservation()
        {
            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    Map[x, y].FillTileEncoding(ref _tileEncoding);
                    int baseIndex = 13 * (x * Map.GetLength(1) + y);
                    for (int i = 0; i < 13; i++)
                    {
                        _mapObservation[baseIndex + i] = _tileEncoding[i];
                    }
                }
            }

            return _mapObservation;
        }

        private const int UnitEncodingSize = 6;
        private const int _maxUnits = 100;
        private int[] _unitEncoding = new int[UnitEncodingSize];
        readonly int[] _unitsObservation = new int[_maxUnits * UnitEncodingSize];
        private int[] GetUnitsObservation()
        {
            int index = 0;
            foreach (var unit in _spawn.ActiveUnits)
            {
                unit.GetUnitEncoding(ref _unitEncoding);
                Array.Copy(_unitEncoding, 0, _unitsObservation, index, _unitEncoding.Length);
                index += _unitEncoding.Length;
            }

            for (int i = index; i < _unitsObservation.Length - _unitEncoding.Length; i++)
            {
                _unitsObservation[i] = 0;
            }

            return _unitsObservation;
        }

        private const int _maxBuildQueueSize = 100;
        readonly int[] _buildQueueObservation = new int[_maxBuildQueueSize * UnitEncodingSize];
        private int[] BuildQueueObservation()
        {
            int index = 0;
            foreach (var unit in _spawn.BuildQueue)
            {
                unit.GetUnitEncoding(ref _unitEncoding);
                Array.Copy(_unitEncoding, 0, _buildQueueObservation, index, _unitEncoding.Length);
                index += _unitEncoding.Length;
                if(index >= _buildQueueObservation.Length)
                    break;
            }

            for (int i = index; i < _buildQueueObservation.Length - _unitEncoding.Length; i++)
            {
                _buildQueueObservation[i] = 0;
            }

            return _buildQueueObservation;
        }

        private float GetCastleObservation()
        {
            return Castle.Health * 1f / Castle.MaxHealth;
        }

        public void ResetGame()
        {
            Castle.ResetCastle();
            
            foreach (var mapTile in Map)
            {
                if (mapTile.Tower == null) continue;

                DestroyImmediate(mapTile.Tower.gameObject);
                mapTile.Type = TileType.Empty;
            }

            DefenderController.Reset();
            AttackerController.Reset();

            _spawn.StartWave();
        }
    }
}