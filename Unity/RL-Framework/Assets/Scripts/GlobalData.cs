using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class GlobalData : MonoBehaviour
    {
        private const string LogsFolderPath = @"Logs\WinRates\";
        public static int AttackerWins = 0;
        public static int DefenderWins = 0;
        private readonly List<string> _logs = new List<string>();
        private TextMeshProUGUI _text;
        private float _timeSinceLastLog;
        private const float LogInterval = 1.0f;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
            TimeController.Instance.OnNextFrame += OnUpdate;
        }

        private void OnUpdate(FramesUpdate f)
        {
            if (DefenderWins + AttackerWins == 0) return;
            var winRatio = 100f * AttackerWins / (DefenderWins + AttackerWins);

            _text.text = $"Attacker win ratio: {winRatio:F2}%";

            _timeSinceLastLog += Time.deltaTime;
            if (!(_timeSinceLastLog > LogInterval)) return;
            _timeSinceLastLog = 0;
            _logs.Add($"{AttackerWins}:{DefenderWins} = {100f * AttackerWins / (DefenderWins + AttackerWins):F2}");
        }

        private void OnDisable()
        {
            LogWinRatioToFile();
        }

        private void LogWinRatioToFile()
        {
            try
            {
                if(!Directory.Exists(LogsFolderPath))
                    Directory.CreateDirectory(LogsFolderPath);

                var filePath = LogsFolderPath + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
                StringBuilder sb = new StringBuilder();
                foreach (var log in _logs)
                    sb.AppendLine(log);
                File.WriteAllText(filePath, sb.ToString());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
