using UnityEngine;
using TMPro; 

public class FPSCounter : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI _fpsText;

    [Header("Settings")]
    public float _updateInterval = 0.2f;

    private float _accumTime = 0f;
    private int _framesCount = 0;
    private float _timeLeft;

    void Start()
    {
        if (_fpsText == null)
        {
            _fpsText = GetComponent<TextMeshProUGUI>();
        }

        _timeLeft = _updateInterval;
    }

    void Update()
    {
        if (_fpsText == null) return;

        _timeLeft -= Time.deltaTime;
        _accumTime += Time.unscaledDeltaTime;
        _framesCount++;

        if (_timeLeft <= 0.0f)
        {
            float fps = _framesCount / _accumTime;

            _fpsText.text = string.Format("{0:F0} FPS", fps);

            if (fps >= 55)
            {
                _fpsText.color = Color.green; 
            }
            else if (fps >= 30)
            {
                _fpsText.color = Color.yellow;
            }
            else
            {
                _fpsText.color = Color.red; 
            }

            _timeLeft = _updateInterval;
            _accumTime = 0.0f;
            _framesCount = 0;
        }
    }
} 