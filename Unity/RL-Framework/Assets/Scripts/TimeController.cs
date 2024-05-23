using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class TimeController : MonoBehaviour
    {
        public static TimeController Instance { get; private set; }
        public event Action<FramesUpdate> OnNextFrame;

        public int FramesPerUpdate = 1;
        public float CustomTimeScale = 1.0f;

        private const int FrameRate = 60;
        private float _timePerFrame;

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

            _timePerFrame = 1f / FrameRate;
            Debug.Log($"Time per frame:{_timePerFrame * FramesPerUpdate / CustomTimeScale}");
            StartCoroutine(FrameUpdater());
        }

        private IEnumerator FrameUpdater()
        {
            while (true)
            {
                OnNextFrame?.Invoke(new FramesUpdate(FramesPerUpdate));
                yield return new WaitForSeconds(_timePerFrame / CustomTimeScale);
            }
        }
    }
}