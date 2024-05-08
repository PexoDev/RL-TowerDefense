using Assets.Scripts.Players.Attacker;
using Assets.Scripts.Players.Defender;
using Assets.Scripts.Units;
using UnityEngine;

#pragma warning disable CS0649 // Assigned via Unity Inspector

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {

        private static GameController _instance;
        public static GameController Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindAnyObjectByType<GameController>();
                return _instance;
            }

            set 
            { 
                if (_instance == null) 
                    _instance = value; 
            }
        }

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
                _spawn.Waves = _waves;
            }
        }

        public float TimeScale = 1f;

        [SerializeField] private GameObject _unitPrefab;
        [SerializeField] private UnitData[] _units;
        [SerializeField] private Wave[] _waves;

        [SerializeField] private GameObject _towerPrefab;
        [SerializeField] private TowerData[] _towers;


        private IDefenderController _defenderController;
        private IAttackerController _attackerController;

        public static void GameOver(bool defenderWon)
        {
            Debug.Log(defenderWon ? "Game Over!\n Defender Won!" : "Game Over!\n Attacker Won!");
        }

        private void Awake()
        {
            TimeController.Instance.CustomTimeScale = TimeScale;

            _defenderController = new RLDefenderController();
            var defenderEconomyManager = gameObject.AddComponent<EconomyManager>();
            _defenderController.Initialize(_towers, defenderEconomyManager, _towerPrefab);

            _attackerController = new RLAttackerController();
            var attackerEconomyManager = gameObject.AddComponent<EconomyManager>();
            _attackerController.Initialize(_units, attackerEconomyManager, Spawn);

            Castle.OnCastleHit += dmg => attackerEconomyManager.Gold += dmg;
            _spawn.OnUnitDeath += unit => defenderEconomyManager.Gold += unit.Data.Reward;
        }

        private void Start()
        {
            _spawn.StartWave();
        }

        public void NextWave(int waveIndex)
        {
            _defenderController.EconomyManager.Gold += 100 * waveIndex;
            _attackerController.EconomyManager.Gold += 100 * waveIndex;
        }

        public DefenderObservation GetDefenderObservation()
        {
            return new DefenderObservation()
            {

            };
        }

        public AttackerObservation GetAttackerObservation()
        {
            return new AttackerObservation()
            {

            };
        }
    }
}
