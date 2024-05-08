using System;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Units;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public int WaveNumber { get; private set; } = 0;
    public Wave[] Waves;
    private const int FramesBetweenEnemies = 5;
    private int framesSinceLastSpawn = 0;
    private bool readyToSpawn = true;
    private readonly Queue<Unit> buildQueue = new Queue<Unit>();
    private readonly List<Unit> activeUnits = new List<Unit>();
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
        framesSinceLastSpawn += framesUpdate.FrameCount;
        if (!readyToSpawn && framesSinceLastSpawn >= FramesBetweenEnemies)
        {
            framesSinceLastSpawn -= FramesBetweenEnemies;
            readyToSpawn = true;
        }

        if (!readyToSpawn || buildQueue.Count <= 0) return;
        if (!(framesSinceLastSpawn >= buildQueue.Peek().Data.BuildTime)) return;

        Unit enemyToSpawn = buildQueue.Dequeue();
        enemyToSpawn.gameObject.SetActive(true);
        activeUnits.Add(enemyToSpawn);
        
        enemyToSpawn.OnDeath += HandleUnitDeath;

        framesSinceLastSpawn -= (int)enemyToSpawn.Data.BuildTime;
        readyToSpawn = false;
        UpdateQueue(new FramesUpdate(0));
    }

    private void HandleUnitDeath(Unit u)
    {
        OnUnitDeath(u);
        activeUnits.Remove(u);
        if (activeUnits.Count == 0 && buildQueue.Count == 0)
        {
            StartWave();
        }
    }

    public void QueueEnemy(UnitData unitData)
    {
        var newEnemy = Instantiate(UnitPrefab, transform.position, Quaternion.identity);
        newEnemy.gameObject.SetActive(false);
        var unit = newEnemy.GetComponent<Unit>();
        unit.Data = unitData;
        buildQueue.Enqueue(unit);
    }

    public void StartWave()
    {
        if (WaveNumber >= Waves.Length)
        {
            GameController.GameOver(true);
            return;
        }

        foreach (var unitData in Waves[WaveNumber].Units)
        {
            QueueEnemy(unitData);
        }

        WaveNumber++;
        GameController.Instance.NextWave(WaveNumber);
    }

    public event Action<Unit> OnUnitDeath = unit => {};
}