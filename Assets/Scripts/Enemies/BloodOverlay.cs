using UnityEngine;
using UnityEngine.UI;

public class BloodOverlay : MonoBehaviour
{
    private Image _image;
    [SerializeField] private float _fadeSpeed = 5f;
    [SerializeField] private float _maxAlpha = 0.8f;

    private float _targetAlpha = 0f; 

    void Awake()
    {
        _image = GetComponent<Image>();
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
    }

    void Update()
    {
        if (Mathf.Abs(_image.color.a - _targetAlpha) > 0.001f)
        {
            Color c = _image.color;
            c.a = Mathf.Lerp(c.a, _targetAlpha, Time.deltaTime * _fadeSpeed);
            _image.color = c;
        }
    }

    public void ShowBloodFlash()
    {
        Color c = _image.color;
        c.a = _maxAlpha; 
        _image.color = c;
    }

    public void UpdateBloodState(int currentHealth, int maxHealth)
    {
        float healthPercent = (float)currentHealth / maxHealth;
        _targetAlpha = (1f - healthPercent) * _maxAlpha;
    }
}