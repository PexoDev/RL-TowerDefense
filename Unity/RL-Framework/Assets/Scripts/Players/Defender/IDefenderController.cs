using Assets.Scripts.Players.Attacker;
using UnityEngine;

namespace Assets.Scripts.Players.Defender
{
    public interface IDefenderController
    {
        public EconomyManager EconomyManager { get; set; }
        void Initialize(TowerData[] towers, EconomyManager economyManager, GameObject towerPrefab);
        void ProcessFrame();
        bool PlaceTower(TowerData towerData, MapTile tile);
        bool SellTower(MapTile tile);
        DefenderObservation GetObservation();
    }
}