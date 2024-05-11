using Assets.Scripts;
using Assets.Scripts.Players.Defender;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class DefenderAgent : Agent
{
    public RLDefenderController DefenderController;
    private bool _firstWave = true;
    private bool _gameOver = false;
    void Start()
    {
        TimeController.Instance.OnNextFrame += MakeDecisionRequest;
        
    }

    void OnDestroy()
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
        if (!_firstWave)
            DefenderController.GameController.ResetGame();

        _gameOver = false;
        _firstWave = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var observations = DefenderController.GameController.GetDefenderObservation();
        
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
            case 0: // Place Tower
                TowerData tower = DefenderController.Towers[secondaryAction];
                MapTile placementTile = DefenderController.GetTileByIndex(tertiaryAction);
                DefenderController.PlaceTower(tower, placementTile);
                break;
            case 1: // Sell Tower
                MapTile towerTile = DefenderController.GetTileByIndex(tertiaryAction);
                DefenderController.SellTower(towerTile);
                AddReward(-10f);
                break;
            case 2: // Buy Worker
                DefenderController.BuyWorker();
                break;
            case 3: // No-Op
                break;
        }

        AddRewards();
    }

    private void AddRewards()
    {
        if(_gameOver) return;

        if (DefenderController.Castle.Health < DefenderController.CastlePreviousHealth)
        {
            AddReward(-0.1f * (DefenderController.CastlePreviousHealth - DefenderController.Castle.Health) / Castle.MaxHealth);
            DefenderController.CastlePreviousHealth = DefenderController.Castle.Health;
        }

        if (DefenderController.WaveClearedWithoutDamage)
        {
            AddReward(1.0f);
            DefenderController.WaveClearedWithoutDamage = false;
        }

        if (DefenderController.Victory)
        {
            _gameOver = true;
            AddReward(10.0f);
            Debug.Log("End episode - Defender Won");
            EndEpisode();
        }

        if (DefenderController.Defeat)
        {
            _gameOver = true;
            AddReward(-10.0f);
            Debug.Log("End episode - Defender Lost");
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = 3; // Default No-Op
    }
}