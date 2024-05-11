using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Units;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameController GameController;
    public int WaveNumber { get; private set; } = 0;
    public Wave[] WavesEnemies;
    public const int Waves = 5;
    public const int WaveMaxFrames = 60 * 30;
    private int _waveFrames = 0;
    private const int FramesBetweenEnemies = 5;
    private int framesSinceLastSpawn = 0;
    private bool readyToSpawn = true;
    public readonly Queue<Unit> BuildQueue = new();
    public readonly List<Unit> ActiveUnits = new();
    public GameObject UnitPrefab { get; set; }

    void OnEnable()
    {
        TimeController.Instance.OnNextFrame += UpdateQueue;
    }

    void OnDisable()
    {
        TimeController.Instance.OnNextFrame -= UpdateQueue;
    }

    private void UpdateQueue(FramesUpdate framesUpdate)
    {
        _waveFrames += framesUpdate.FrameCount;
        framesSinceLastSpawn += framesUpdate.FrameCount;
        if (!readyToSpawn && framesSinceLastSpawn >= FramesBetweenEnemies)
        {
            framesSinceLastSpawn -= FramesBetweenEnemies;
            readyToSpawn = true;
        }

        if (!readyToSpawn || BuildQueue.Count <= 0) return;
        if (!(framesSinceLastSpawn >= BuildQueue.Peek().Data.BuildTime)) return;

        Unit enemyToSpawn = BuildQueue.Dequeue();
        enemyToSpawn.GameController = GameController;
        enemyToSpawn.gameObject.SetActive(true);
        ActiveUnits.Add(enemyToSpawn);
        
        enemyToSpawn.OnDeath += HandleUnitDeath;

        framesSinceLastSpawn -= (int)enemyToSpawn.Data.BuildTime;
        readyToSpawn = false;
        
        if ((ActiveUnits.Count < 1 && BuildQueue.Count < 1) || _waveFrames >= WaveMaxFrames)
        {
            StartWave();
        }

        UpdateQueue(new(0));
    }

    private void HandleUnitDeath(Unit u)
    {
        OnUnitDeath(u);
        ActiveUnits.Remove(u);
        if (ActiveUnits.Count == 0 && BuildQueue.Count == 0)
        {
            StartWave();
        }
    }

    public void QueueEnemy(UnitData unitData)
    {
        var newEnemy = Instantiate(UnitPrefab, transform.position, Quaternion.identity, transform);
        newEnemy.gameObject.SetActive(false);
        var unit = newEnemy.GetComponent<Unit>();
        unit.Data = unitData;
        BuildQueue.Enqueue(unit);
    }

    public void StartWave()
    {
        _waveFrames = 0;

        if (WaveNumber >= Waves)
        {
            GameController.GameOver(true);
            return;
        }

        foreach (var unitData in WavesEnemies[WaveNumber].Units())
        {
            QueueEnemy(unitData);
        }

        WaveNumber++;
        GameController.NextWave(WaveNumber);
    }

    public event Action<Unit> OnUnitDeath = unit => {};

    public void Reset()
    {
        WaveNumber = 0;
        foreach (var unit in ActiveUnits)
            DestroyImmediate(unit.gameObject);
        BuildQueue.Clear();
        ActiveUnits.Clear();

        framesSinceLastSpawn = 0;
        readyToSpawn = true;
    }
}