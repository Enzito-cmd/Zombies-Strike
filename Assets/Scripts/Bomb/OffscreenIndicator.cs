using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    public static OffScreenIndicator Instance;

    [Header("References")]
    public RectTransform _arrowRect;
    public Image _arrowImage;

    [Header("Config")]
    public float _borderOffset = 50f;

    private Camera _mainCamera;
    private Transform _targetBomb;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _mainCamera = Camera.main;

        if (_arrowRect == null) _arrowRect = GetComponent<RectTransform>();
        if (_arrowImage == null) _arrowImage = GetComponent<Image>();

        _arrowImage.enabled = false;
    }

    public void SetTarget(Transform newTarget)
    {
        _targetBomb = newTarget;
    }

    void Update()
    {
        if (_targetBomb == null)
        {
            _arrowImage.enabled = false;
            return;
        }

        Vector3 screenPos = _mainCamera.WorldToScreenPoint(_targetBomb.position);
        bool isOffScreen = screenPos.z < 0 || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;

        if (isOffScreen)
        {
            _arrowImage.enabled = true;

            if (screenPos.z < 0)
            {
                screenPos.x = Screen.width - screenPos.x;
                screenPos.y = Screen.height - screenPos.y;
            }

            float targetX = (screenPos.x < Screen.width / 2f) ? _borderOffset : Screen.width - _borderOffset;
            float targetY = Mathf.Clamp(screenPos.y, _borderOffset, Screen.height - _borderOffset);

            _arrowRect.position = new Vector3(targetX, targetY, 0);

            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            Vector3 direction = (screenPos - screenCenter).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _arrowRect.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            _arrowImage.enabled = false;
        }
    }
}