using Assets.Scripts.Units;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace Assets.Scripts.Players.Attacker
{
    public class AttackerAgent : Agent
    {
        public RLAttackerController AttackerController;
        private bool _gameOver;

        private void Start()
        {
            TimeController.Instance.OnNextFrame += MakeDecisionRequest;

        }

        private void OnDestroy()
        {
            TimeController.Instance.OnNextFrame -= MakeDecisionRequest;
        }

        private void MakeDecisionRequest(FramesUpdate obj)
        {
            if (AttackerController == null) return;
            if (this.isActiveAndEnabled)
            {
                RequestDecision();
            }
        }

        public override void OnEpisodeBegin()
        {
            _gameOver = false;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            var observations = AttackerController.GetObservation();

            foreach (var o in observations.MapObservation)
                sensor.AddObservation(o);

            foreach (var o in observations.UnitsObservation)
                sensor.AddObservation(o);

            foreach (var o in observations.BuildQueueObservation)
                sensor.AddObservation(o);

            sensor.AddObservation(observations.EconomyObservation);
            sensor.AddObservation(observations.CastleObservation);
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            base.OnActionReceived(actionBuffers);

            int mainAction = actionBuffers.DiscreteActions[0];
            int secondaryAction = actionBuffers.DiscreteActions[1];

            switch (mainAction)
            {
                case 0:
                    UnitData unit = AttackerController.Units[secondaryAction];
                    AttackerController.SpawnUnit(unit);
                    break;
                case 1:
                    AttackerController.BuyWorker();
                    break;
                case 2: // No-Op
                    break;
            }

            AddRewards();
        }

        private void AddRewards()
        {
            if (_gameOver) return;

            if (AttackerController.Castle.Health < AttackerController.CastlePreviousHealth)
            {
                AddReward(0.1f * (AttackerController.CastlePreviousHealth - AttackerController.Castle.Health) / Castle.MaxHealth);
                AttackerController.CastlePreviousHealth = AttackerController.Castle.Health;
            }

            if (AttackerController.Victory)
            {
                _gameOver = true;
                AddReward(5f);
                AddReward(AttackerController.EconomyManager.Gold * 0.01f);
                EndEpisode();
            }

            if (AttackerController.Defeat)
            {
                _gameOver = true;
                AddReward(-5f);
                EndEpisode();
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;
            discreteActionsOut[0] = 2; // Default No-Op
        }
    }
}