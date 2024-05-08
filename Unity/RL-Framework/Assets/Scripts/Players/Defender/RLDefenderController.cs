using Assets.Scripts.Map;
using UnityEngine;

namespace Assets.Scripts.Players.Defender
{
    public class RLDefenderController : IDefenderController
    {
        public TowerData[] Towers { get; set; }
        public EconomyManager EconomyManager { get; set; }
        private GameObject _towerPrefab;

        public void Initialize(TowerData[] towers, EconomyManager economyManager, GameObject towerPrefab)
        {
            Towers = towers;
            EconomyManager = economyManager;
            _towerPrefab = towerPrefab;

            MOCK();
        }

        private void MOCK()
        {
            EconomyManager.Gold = 1000000;
            for (int i = 0; i < 10; i++)
            {
                PlaceTower(Towers[Random.Range(0, Towers.Length)], GetRandomMapTile());
            }
            MapTile GetRandomMapTile()
            {
                return GameController.Instance.Map[Random.Range(0, GameController.Instance.Map.GetLength(0)), Random.Range(0, GameController.Instance.Map.GetLength(1))];
            }

        }

        public void ProcessFrame()
        {
            throw new System.NotImplementedException();
        }

        public bool PlaceTower(TowerData tower, MapTile tile)
        {
            if (EconomyManager.Gold > tower.Cost)
                EconomyManager.Gold -= tower.Cost;
            else
                return false;

            tile.Type = TileType.Tower;
            var towerObject = GameObject.Instantiate(_towerPrefab, tile.transform);
            var towerInstance = towerObject.GetComponent<Tower>();
            towerInstance.Initialize(tower);
            return true;
        }

        public bool SellTower(MapTile tile)
        {
            if(tile.Type != TileType.Tower)
                return false;
            var tower = tile.GetComponentInChildren<Tower>();
            if(tower == null)
                return false;

            EconomyManager.Gold += tower.Data.Cost / 2;
            GameObject.Destroy(tower.gameObject);
            tile.Type = TileType.Empty;
            return true;
        }

        public bool BuyWorker()
        {
            if(EconomyManager.Gold < EconomyManager.WorkerCost)
                return false;
            EconomyManager.BuyWorker();
            return true;
        }

        public DefenderObservation GetObservation()
        {
            return GameController.Instance.GetDefenderObservation();
        }
    }
}