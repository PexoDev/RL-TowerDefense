using System.Collections;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private TextMeshProUGUI _fpsText;
    void Start()
    {
        _fpsText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        _fpsText.text = $"FPS: {1 / Time.deltaTime:0}";
    }
}
