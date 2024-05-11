using UnityEngine;
using System;
using System.Collections;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance { get; private set; }
    public event Action<FramesUpdate> OnNextFrame;

    private const int FrameRate = 60;
    private float timePerFrame;
    public float customTimeScale = 1.0f;

    public float CustomTimeScale
    {
        get => customTimeScale;
        set => customTimeScale = Mathf.Clamp(value, 0.1f, 10.0f);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        timePerFrame = 1f / FrameRate;
        Debug.Log($"Time per frame:{timePerFrame/customTimeScale}");
        StartCoroutine(FrameUpdater());
    }

    IEnumerator FrameUpdater()
    {
        while (true)
        {
            OnNextFrame?.Invoke(new FramesUpdate(1));
            yield return new WaitForSeconds(timePerFrame / customTimeScale);
        }
    }

    public void IncreaseSpeed()
    {
        CustomTimeScale *= 2;
    }

    public void DecreaseSpeed()
    {
        CustomTimeScale /= 2;
    }
}