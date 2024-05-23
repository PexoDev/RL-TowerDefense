using Assets.Scripts.Map;
using Assets.Scripts.Units;

namespace Assets.Scripts.Players
{
    public class ObservationHelper
    {
        public MapTile[,] Map { get; set; }
        public EnemySpawner Spawn { get; set; }
        public Castle Castle { get; set; }

        public AgentObservation GetEnvironmentObservation(EconomyManager economyManager)
        {
            var observation = new AgentObservation()
            {
                MapObservation = GetMapObservation(),
                UnitsObservation = GetUnitsObservation(),
                BuildQueueObservation = BuildQueueObservation(),
                EconomyObservation = GetEconomyObservation(economyManager),
                CastleObservation = GetCastleObservation()
            };

            return observation;
        }

        private float[] GetEconomyObservation(EconomyManager economyManager)
        {
            float[] economyObservation = new float[2];
            economyObservation[0] = economyManager.Gold * 1f / EconomyManager.MaxGold;
            economyObservation[1] = economyManager.NumberOfWorkers * 1f / EconomyManager.MaxWorkers;
            return economyObservation;
        }

        private readonly int[] _mapObservation = new int[MapGenerator.MapSize * MapGenerator.MapSize];
        private int[] GetMapObservation()
        {
            for (int x = 0; x < Map.GetLength(0); x++)
            for (int y = 0; y < Map.GetLength(1); y++)
                _mapObservation[(x * MapGenerator.MapSize) + y] = Map[x, y].GetMapEncoding();

            return _mapObservation;
        }

        private const int UnitEncodingSize = 9;
        private readonly int[] _unitsObservation = new int[UnitEncodingSize];
        private int[] GetUnitsObservation()
        {
            for (int i = 0; i < _unitsObservation.Length; i++)
                _unitsObservation[i] = 0;

            foreach (var unit in Spawn.ActiveUnits)
            {
                var unitIndex = unit.GetUnitEncoding();
                _unitsObservation[unitIndex]++;
            }

            return _unitsObservation;
        }

        private readonly int[] _buildQueueObservation = new int[UnitEncodingSize];
        private int[] BuildQueueObservation()
        {
            for (int i = 0; i < _buildQueueObservation.Length; i++)
                _buildQueueObservation[i] = 0;

            foreach (var unit in Spawn.BuildQueue)
            {
                var unitIndex = unit.GetUnitEncoding();
                _buildQueueObservation[unitIndex]++;
            }

            return _buildQueueObservation;
        }

        private float GetCastleObservation()
        {
            return Castle.Health * 1f / Castle.MaxHealth;
        }
    }
}