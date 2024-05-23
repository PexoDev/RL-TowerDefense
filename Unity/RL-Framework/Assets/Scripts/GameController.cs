using Assets.Scripts.Map;
using Assets.Scripts.Players;
using Assets.Scripts.Players.Attacker;
using Assets.Scripts.Players.Defender;
using Assets.Scripts.Towers;
using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

#pragma warning disable CS0649 // Assigned via Unity Inspector

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        public event Action<int, int> OnWaveEnded = (waveNumber, waveLength) => { };
        public event Action<bool> OnGameOver = defenderWon => { };
        public event Action OnGameReset = () => { };

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
                InitializeUnitSpawner();
            }
        }

        [SerializeField] private GameObject _unitPrefab;
        [SerializeField] private UnitData[] _units;
        [SerializeField] private Wave[] _waves;

        [SerializeField] private GameObject _towerPrefab;
        [SerializeField] private TowerData[] _towers;

        public IDefenderController DefenderController { get; private set; }
        public IAttackerController AttackerController { get; private set; }
        private ObservationHelper _observationHelper;

        public TextMeshProUGUI AttackerGoldText;
        public TextMeshProUGUI AttackerWorkersText;
        public TextMeshProUGUI DefenderGoldText;
        public TextMeshProUGUI DefenderWorkersText;
        public TextMeshProUGUI ResultText;
        public TextMeshProUGUI CurrentWaveText;

        private int _attackerWins = 0;
        private int _defenderWins = 0;

        private void Start()
        {
            InitializeDefenderAgent();
            InitializeAttackerAgent();
            InitializeObservationsHelper();

            _spawn.OnUnitDeath += HandleOnUnitDeath;
            _spawn.StartWave();
        }

        public void GameOver(bool defenderWon)
        {
            OnGameOver(defenderWon);

            if (defenderWon)
            {
                _defenderWins++;
                GlobalData.DefenderWins++;
            }
            else
            {
                _attackerWins++;
                GlobalData.AttackerWins++;
            }

            ResultText.text = $"{_attackerWins} - {_defenderWins}";
        }

        public void NextWave(int waveIndex, int waveLengthInFrames)
        {
            OnWaveEnded(waveIndex, waveLengthInFrames);
            DefenderController.EconomyManager.Gold += 100 * waveIndex;
            AttackerController.EconomyManager.Gold += 100 * waveIndex;
            CurrentWaveText.text = $"Wave: {waveIndex}";
        }
        
        public void ResetGame()
        {
            OnGameReset();

            foreach (var mapTile in Map)
            {
                if (mapTile.Tower == null) continue;

                DestroyImmediate(mapTile.Tower.gameObject);
                mapTile.Type = TileType.Empty;
            }

            DefenderController.Reset();
            AttackerController.Reset();
            AttackerController.EnemySpawner = _spawn;
            DefenderController.Castle = Castle;
            AttackerController.Castle = Castle;
            _spawn.OnUnitDeath += (unit, reachedCastle) =>
            {
                if (reachedCastle) return;

                DefenderController.EconomyManager.Gold += unit.Data.Reward;
                DefenderController.UnitsKilled += unit.Data.Reward;
            };
            _spawn.StartWave();
        }

        public AgentObservation GetEnvironmentObservation(EconomyManager economyManager) => _observationHelper.GetEnvironmentObservation(economyManager);

        private void InitializeAttackerAgent()
        {
            AttackerController = new RLAttackerController();
            var attackerEconomyManager = gameObject.AddComponent<EconomyManager>();
            attackerEconomyManager.OnGoldUpdate += gold => AttackerGoldText.text = $"Attacker: {gold}";
            attackerEconomyManager.OnWorkersUpdate += workers => AttackerWorkersText.text = $"Workers: {workers}";
            AttackerController.Initialize(_units, attackerEconomyManager, Spawn, this);
            var attackerAgent = GetComponentInChildren<AttackerAgent>();
            if (attackerAgent != null)
                attackerAgent.AttackerController = (RLAttackerController)AttackerController;
        }

        private void InitializeDefenderAgent()
        {
            DefenderController = new RLDefenderController();
            var defenderEconomyManager = gameObject.AddComponent<EconomyManager>();
            defenderEconomyManager.OnGoldUpdate += gold => DefenderGoldText.text = $"Defender: {gold}";
            defenderEconomyManager.OnWorkersUpdate += workers => DefenderWorkersText.text = $"Workers: {workers}";
            DefenderController.Initialize(_towers, defenderEconomyManager, _towerPrefab, this);
            var defenderAgent = GetComponentInChildren<DefenderAgent>();
            if (defenderAgent != null)
                defenderAgent.DefenderController = (RLDefenderController)DefenderController;
        }
        
        private void InitializeObservationsHelper()
        {
            _observationHelper = new ObservationHelper
            {
                Map = Map,
                Spawn = Spawn,
                Castle = Castle
            };
        }

        private void HandleOnUnitDeath(Unit unit, bool reachedCastle)
        {
            if (reachedCastle) return;
            DefenderController.EconomyManager.Gold += unit.Data.Reward;
            DefenderController.UnitsKilled += unit.Data.Reward;
        }

        private void InitializeUnitSpawner()
        {
            _spawn.UnitPrefab = _unitPrefab;
            List<Wave> waves = new();
            foreach (Wave w in _waves)
                waves.Add(Instantiate(w));
            _spawn.WavesEnemies = waves.ToArray();
        }
    }
}