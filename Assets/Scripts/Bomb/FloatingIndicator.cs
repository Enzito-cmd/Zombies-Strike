using UnityEngine;

public class FloatingIndicator : MonoBehaviour
{
    [Header("Animation settings")]
    public float _spinSpeed = 100f; 
    public float _floatAmplitude = 0.2f;
    public float _floatFrequency = 2f; 

    private Vector3 _startPos;

    void Start()
    {
        _startPos = transform.localPosition;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, _spinSpeed * Time.deltaTime, Space.World);

        float newY = _startPos.y + (Mathf.Sin(Time.time * _floatFrequency) * _floatAmplitude);
        transform.localPosition = new Vector3(_startPos.x, newY, _startPos.z);
    }
}