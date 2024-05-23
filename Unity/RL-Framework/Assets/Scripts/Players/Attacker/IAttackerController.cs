using Assets.Scripts.Units;

namespace Assets.Scripts.Players.Attacker
{
    public interface IAttackerController
    {
        public Castle Castle { get; set; }
        public UnitData[] Units { get; set; }
        public EconomyManager EconomyManager { get; set; }
        public EnemySpawner EnemySpawner { get; set; }

        void Initialize(UnitData[] units, EconomyManager economyManager, EnemySpawner enemySpawner, GameController gameController);
        bool SpawnUnit(UnitData unitData);
        bool BuyWorker();
        AgentObservation GetObservation();
        void Reset();
    }
}