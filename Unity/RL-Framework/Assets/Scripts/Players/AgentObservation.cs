namespace Assets.Scripts.Players
{
    public class AgentObservation
    {
        public int[] MapObservation { get; set; }
        public int[] UnitsObservation { get; set; }
        public int[] BuildQueueObservation { get; set; }
        public float[] EconomyObservation { get; set; }
        public float CastleObservation { get; set; }
    }
}