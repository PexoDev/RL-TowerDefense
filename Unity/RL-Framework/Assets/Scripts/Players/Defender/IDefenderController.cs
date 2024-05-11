using UnityEngine;

namespace Assets.Scripts.Players.Defender
{
    public interface IDefenderController
    {
        public EconomyManager EconomyManager { get; set; }
        void Initialize(TowerData[] towers, EconomyManager economyManager, GameObject towerPrefab, GameController gameController);
        bool PlaceTower(TowerData towerData, MapTile tile);
        bool SellTower(MapTile tile);
        DefenderObservation GetObservation();
        void Reset();
    }
}