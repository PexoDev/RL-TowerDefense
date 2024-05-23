using Assets.Scripts.Map;
using Assets.Scripts.Towers;
using UnityEngine;

namespace Assets.Scripts.Players.Defender
{
    public class RLDefenderController : IDefenderController
    {
        public GameController GameController { get; private set; }
        public EconomyManager EconomyManager { get; set; }
        public TowerData[] Towers { get; set; }
        public Castle Castle { get; set; }

        public int CastlePreviousHealth { get; set; }
        public int CastleHealthOnStartOfTheWave { get; set; }

        public bool WaveClearedWithoutDamage { get; set; }
        public int WaveClearTime { get; set; } = -1;
        public bool WaveCleared { get; set; }
        public int WaveNumber { get; set; }
        public int UnitsKilled { get; set; }

        public bool Victory { get; set; }
        public bool Defeat { get; set; }


        private GameObject _towerPrefab;

        public void Initialize(TowerData[] towers, EconomyManager economyManager, GameObject towerPrefab, GameController gameController)
        {
            GameController = gameController;
            Towers = towers;
            EconomyManager = economyManager;
            _towerPrefab = towerPrefab;

            Castle = GameController.Castle;
            CastleHealthOnStartOfTheWave = Castle.Health;
            CastlePreviousHealth = Castle.Health;

            GameController.OnWaveEnded += (waveIndex, waveLengthInFrames) =>
            {
                WaveCleared = true;
                WaveClearTime = waveLengthInFrames;
                WaveNumber = waveIndex;

                if (Castle.Health == CastleHealthOnStartOfTheWave)
                    WaveClearedWithoutDamage = true;
                CastleHealthOnStartOfTheWave = Castle.Health;
            };

            GameController.OnGameOver += defenderWon =>
            {
                if (defenderWon)
                    Victory = true;
                else
                    Defeat = true;
            };
        }

        public bool PlaceTower(TowerData tower, MapTile tile)
        {
            if (tile.Type != TileType.Empty)
                return false;

            if (EconomyManager.Gold < tower.Cost)
                return false;
            EconomyManager.Gold -= tower.Cost;

            tile.Type = TileType.Tower;
            var towerObject = GameObject.Instantiate(_towerPrefab, tile.transform);
            var towerInstance = towerObject.GetComponent<Tower>();
            tile.Tower = towerInstance;
            towerInstance.Initialize(tower);
            return true;
        }

        public bool SellTower(MapTile tile)
        {
            if (tile.Type != TileType.Tower)
                return false;
            var tower = tile.GetComponentInChildren<Tower>();
            if (tower == null)
                return false;

            EconomyManager.Gold += tower.Data.Cost / 2;
            GameObject.Destroy(tower.gameObject);
            tile.Type = TileType.Empty;
            return true;
        }

        public bool BuyWorker()
        {
            if (EconomyManager.Gold < EconomyManager.WorkerCost || EconomyManager.NumberOfWorkers >= EconomyManager.MaxWorkers)
                return false;
            EconomyManager.BuyWorker();
            return true;
        }

        public AgentObservation GetObservation()
        {
            return GameController.GetEnvironmentObservation(EconomyManager);
        }

        public void Reset()
        {
            EconomyManager.Reset();
            Defeat = false;
            Victory = false;
            WaveCleared = false;
            WaveNumber = 0;
            UnitsKilled = 0;
            CastlePreviousHealth = Castle.MaxHealth;
            CastleHealthOnStartOfTheWave = Castle.MaxHealth;
            WaveClearedWithoutDamage = false;
        }

        public MapTile GetTileByIndex(int index)
        {
            if (index is < 0 or >= MapGenerator.MapSize * MapGenerator.MapSize)
            {
                return null;
            }

            int x = index / MapGenerator.MapSize;
            int y = index % MapGenerator.MapSize;

            return GameController.Map[x, y];
        }
    }
}