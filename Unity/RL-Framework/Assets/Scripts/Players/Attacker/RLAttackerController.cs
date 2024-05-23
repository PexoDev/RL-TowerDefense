using Assets.Scripts.Units;

namespace Assets.Scripts.Players.Attacker
{
    public class RLAttackerController : IAttackerController
    {
        public GameController GameController { get; set; }
        public EnemySpawner EnemySpawner { get; set; }
        public EconomyManager EconomyManager { get; set; }
        public Castle Castle { get; set; }
        public UnitData[] Units { get; set; }

        public int CastlePreviousHealth { get; set; }
        public int CastleHealthOnStartOfTheWave { get; set; }

        public bool WaveClearedWithoutDamage { get; set; }
        public int WaveClearTime { get; set; } = -1;
        public bool WaveCleared { get; set; }
        public int WaveNumber { get; set; }

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

            GameController.OnWaveEnded += (waveIndex, waveLengthInFrames) =>
            {
                if (Castle.Health == CastleHealthOnStartOfTheWave)
                    WaveClearedWithoutDamage = true;
                CastleHealthOnStartOfTheWave = Castle.Health;
                WaveClearTime += waveLengthInFrames;
                WaveCleared = true;
                WaveNumber = waveIndex;
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
            if (EconomyManager.Gold > unitData.Cost)
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

        public AgentObservation GetObservation()
        {
            return GameController.GetEnvironmentObservation(EconomyManager);
        }

        public void Reset()
        {
            Defeat = false;
            Victory = false;
            EconomyManager.Reset();
        }
    }
}