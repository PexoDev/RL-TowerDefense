namespace Assets.Scripts.Players.Attacker
{
    public class RLAttackerController : IAttackerController
    {
        public UnitData[] Units { get; set; }
        public EconomyManager EconomyManager { get; set; }
        public EnemySpawner EnemySpawner { get; set; }

        public void Initialize(UnitData[] units, EconomyManager economyManager, EnemySpawner enemySpawner)
        {
            Units = units;
            EconomyManager = economyManager;
            EnemySpawner = enemySpawner;
        }

        public void ProcessFrame()
        {
            throw new System.NotImplementedException();
        }

        public bool SpawnUnit(UnitData unitData)
        {
            if(EconomyManager.Gold > unitData.Cost)
                EconomyManager.Gold -= unitData.Cost;
            else
                return false;

            EnemySpawner.QueueEnemy(unitData);
            return true;
        }

        public bool BuyWorker()
        {
            if (EconomyManager.Gold < EconomyManager.WorkerCost)
                return false;
            EconomyManager.BuyWorker();
            return true;
        }

        public AttackerObservation GetObservation()
        {
            return GameController.Instance.GetAttackerObservation();
        }
    }
}