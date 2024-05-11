namespace Assets.Scripts.Players.Attacker
{
    public class RLAttackerController : IAttackerController
    {
        public GameController GameController { get; set; }
        public UnitData[] Units { get; set; }
        public EconomyManager EconomyManager { get; set; }
        public EnemySpawner EnemySpawner { get; set; }

        public Castle Castle { get; set; }
        public int CastlePreviousHealth { get; set; }
        public int CastleHealthOnStartOfTheWave { get; set; }
        public bool WaveClearedWithoutDamage { get; set; }
        public bool Victory { get; set; }
        public bool Defeat { get; set; }


        public void Initialize(UnitData[] units, EconomyManager economyManager, EnemySpawner enemySpawner, GameController gameController)
        {
            Units = units;
            EconomyManager = economyManager;
            EnemySpawner = enemySpawner;
            GameController = gameController;
            Castle = GameController.Castle;
            CastleHealthOnStartOfTheWave = Castle.Health;
            CastlePreviousHealth = Castle.Health;
            GameController.OnWaveEnded += () =>
            {
                if (Castle.Health == CastleHealthOnStartOfTheWave)
                    WaveClearedWithoutDamage = true;
                CastleHealthOnStartOfTheWave = Castle.Health;
            };

            GameController.OnGameOver += defenderWon =>
            {
                if (defenderWon)
                    Defeat = true;
                else
                    Victory = true;
            };
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
            if (EconomyManager.Gold < EconomyManager.WorkerCost || EconomyManager.NumberOfWorkers >= EconomyManager.MaxWorkers)
                return false;
            EconomyManager.BuyWorker();
            return true;
        }

        public AttackerObservation GetObservation()
        {
            return GameController.GetAttackerObservation();
        }

        public void Reset()
        {
            Defeat = false;
            Victory = false;
            EconomyManager.Reset();
            EnemySpawner.Reset();
        }
    }
}