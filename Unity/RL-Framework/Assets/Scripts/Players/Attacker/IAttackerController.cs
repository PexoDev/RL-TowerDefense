namespace Assets.Scripts.Players.Attacker
{
    public interface IAttackerController
    {
        public UnitData[] Units { get; set; }
        public EconomyManager EconomyManager { get; set; }
        public EnemySpawner EnemySpawner { get; set; }

        void Initialize(UnitData[] units, EconomyManager economyManager, EnemySpawner enemySpawner);
        void ProcessFrame();
        bool SpawnUnit(UnitData unitData);
        bool BuyWorker();
        AttackerObservation GetObservation();
    }
}