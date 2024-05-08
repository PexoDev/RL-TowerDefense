using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public int Gold { get; set; }
    public const int WorkerCost = 50;
    public const int IncomePerWorker = 1;

    private int numberOfWorkers;
    private int incomePerWave = 200;
    private int _framesSinceLastUpdate = 0;

    void Start()
    {
        Gold = 20; // Starting Gold
    }

    void OnEnable()
    {
        TimeController.Instance.OnNextFrame += ProcessFrame;
    }

    void OnDisable()
    {
        TimeController.Instance.OnNextFrame -= ProcessFrame;
    }

    private void ProcessFrame(FramesUpdate obj)
    {
        _framesSinceLastUpdate += obj.FrameCount;
        if (_framesSinceLastUpdate >= 60)
        {
            _framesSinceLastUpdate = 0;
            Gold += numberOfWorkers * IncomePerWorker;
        }
    }

    public void BuyWorker()
    {
        if (Gold >= WorkerCost)
        {
            Gold -= WorkerCost;
            numberOfWorkers++;
        }
    }
}