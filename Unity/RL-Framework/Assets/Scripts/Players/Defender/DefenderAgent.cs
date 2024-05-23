using Assets.Scripts.Map;
using Assets.Scripts.Towers;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Assets.Scripts.Players.Defender
{
    public class DefenderAgent : Agent
    {
        public RLDefenderController DefenderController;
        private bool _gameOver = false;

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
            if (DefenderController == null) return;
            if (this.isActiveAndEnabled)
            {
                RequestDecision();
            }
        }

        public override void OnEpisodeBegin()
        {
            DefenderController.GameController.ResetGame();
            _gameOver = false;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            var observations = DefenderController.GetObservation();

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
            int tertiaryAction = actionBuffers.DiscreteActions[2];

            switch (mainAction)
            {
                case 0:
                    TowerData tower = DefenderController.Towers[secondaryAction];
                    MapTile placementTile = DefenderController.GetTileByIndex(tertiaryAction);
                    DefenderController.PlaceTower(tower, placementTile);
                    break;
                case 1:
                    DefenderController.BuyWorker();
                    break;
                case 2: // No-Op
                    break;
            }

            AddRewards();
        }

        private void AddRewards()
        {
            if (_gameOver) return;

            if (DefenderController.Victory)
            {
                _gameOver = true;
                AddReward(5f);
                AddReward(DefenderController.EconomyManager.Gold * 0.05f);
                Debug.Log("End episode - Defender Won");
                EndEpisode();
            }
            if (DefenderController.Defeat)
            {
                _gameOver = true;
                AddReward(-10);
                Debug.Log("End episode - Defender Lost");
                EndEpisode();
            }

            if (DefenderController.UnitsKilled < 1) return;
            AddReward(0.1f * DefenderController.UnitsKilled);
            DefenderController.UnitsKilled = 0;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;
            discreteActionsOut[0] = 2; // Default No-Op
        }
    }
}