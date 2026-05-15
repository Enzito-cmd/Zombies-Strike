using UnityEngine;
using UnityEngine.UI;

public class BloodOverlay : MonoBehaviour
{
    private Image _image;
    [SerializeField] private float _fadeSpeed = 5f; 
    [SerializeField] private float _maxAlpha = 0.8f; 

    void Awake()
    {
        _image = GetComponent<Image>();
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
    }

    void Update()
    {
        if (_image.color.a > 0)
        {
            Color c = _image.color;
            c.a -= Time.deltaTime * _fadeSpeed;
            c.a = Mathf.Max(c.a, 0);
            _image.color = c;
        }
    }

    public void ShowBlood()
    {
        Color c = _image.color;
        c.a = _maxAlpha;
        _image.color = c;
    }
}