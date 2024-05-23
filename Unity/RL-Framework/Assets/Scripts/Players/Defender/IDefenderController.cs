using Assets.Scripts.Map;
using Assets.Scripts.Towers;
using UnityEngine;

namespace Assets.Scripts.Players.Defender
{
    public interface IDefenderController
    {
        public int UnitsKilled { get; set; }
        public Castle Castle { get; set; }
        public EconomyManager EconomyManager { get; set; }
        void Initialize(TowerData[] towers, EconomyManager economyManager, GameObject towerPrefab, GameController gameController);
        bool PlaceTower(TowerData towerData, MapTile tile);
        bool SellTower(MapTile tile);
        AgentObservation GetObservation();
        void Reset();
    }
}