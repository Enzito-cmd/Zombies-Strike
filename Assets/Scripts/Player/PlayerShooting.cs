using UnityEngine;
using StarterAssets;
using System.Collections;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public StarterAssetsInputs _input;
    public TextMeshProUGUI _ammoText;
    public GameObject _impactEffectPrefab;
    public GameObject _bloodImpactPrefab;

    [Header("Active Weapon (ScriptableObject)")]
    public WeaponData _currentWeaponData;

    private int _bulletsInMag;
    private int _ammoReserve;
    private float _nextTimeToFire = 0f;
    private bool _isReloading = false;

    private Camera _mainCamera;
    private Vector3 _originalWeaponPos;
    private GameObject _currentWeaponModel;
    private Transform _dynamicFirePoint;

    void Start()
    {
        _mainCamera = Camera.main;
        _originalWeaponPos = transform.localPosition;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (_currentWeaponData != null)
        {
            ChangeWeapon(_currentWeaponData);
        }
    }

    public void ChangeWeapon(WeaponData newWeapon)
    {
        if (_isReloading) return;

        _currentWeaponData = newWeapon;

        _bulletsInMag = _currentWeaponData._magSize;
        _ammoReserve = _currentWeaponData._maxReserveAmmo;

        if (_currentWeaponModel != null) Destroy(_currentWeaponModel);

        if (_currentWeaponData._weaponModelPrefab != null)
        {
            _currentWeaponModel = Instantiate(_currentWeaponData._weaponModelPrefab, transform);
            _currentWeaponModel.transform.localPosition = Vector3.zero;
            _currentWeaponModel.transform.localRotation = Quaternion.identity;

            Transform foundFirePoint = _currentWeaponModel.transform.Find("FirePoint");
            if (foundFirePoint != null)
            {
                _dynamicFirePoint = foundFirePoint;
            }
            else
            {
                _dynamicFirePoint = transform;
            }
        }

        UpdateAmmoUI();
    }

    void Update()
    {
        if (_input == null || _currentWeaponData == null) return;

        if (_isReloading)
        {
            _input.shoot = false;
            _input.reload = false;
            return;
        }

        if (_input.shoot && _bulletsInMag <= 0 && _ammoReserve > 0)
        {
            _input.shoot = false;
            StartCoroutine(Reload());
            return;
        }

        if (_input.reload)
        {
            _input.reload = false;
            if (_bulletsInMag < _currentWeaponData._magSize && _ammoReserve > 0)
            {
                StartCoroutine(Reload());
            }
        }

        if (_input.shoot && Time.time >= _nextTimeToFire && _bulletsInMag > 0)
        {
            _nextTimeToFire = Time.time + _currentWeaponData._fireRate;
            Shoot();
            _input.shoot = false;
        }
        else if (_input.shoot)
        {
            _input.shoot = false;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, _originalWeaponPos, Time.deltaTime * 10f);
    }

    void Shoot()
    {
        _bulletsInMag--;
        UpdateAmmoUI();

        if (AudioManager._instance != null && _currentWeaponData._shootSound != null)
        {
            AudioManager._instance.PlaySFX(_currentWeaponData._shootSound);
        }

        if (_currentWeaponData._muzzleFlashPrefab != null && _dynamicFirePoint != null)
        {
            GameObject _flash = Instantiate(_currentWeaponData._muzzleFlashPrefab, _dynamicFirePoint.position, _dynamicFirePoint.rotation);
            _flash.transform.SetParent(_dynamicFirePoint);
            _flash.transform.localScale = Vector3.one;
            Destroy(_flash, 0.1f);
        }

        transform.localPosition -= Vector3.forward * _currentWeaponData._recoilAmount;

        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _currentWeaponData._range))
        {
            ZombieHealth _zombie = hit.transform.GetComponent<ZombieHealth>();

            if (hit.transform.CompareTag("Enemy") && _zombie != null)
            {
                _zombie.TakeDamage(_currentWeaponData._damage);

                if (_bloodImpactPrefab != null)
                {
                    GameObject _blood = Instantiate(_bloodImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(_blood, 1f);
                }
            }
            else
            {
                if (_impactEffectPrefab != null)
                {
                    GameObject _impact = Instantiate(_impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(_impact, 1f);
                }
            }
        }
    }

    IEnumerator Reload()
    {
        _isReloading = true;

        Vector3 reloadPos = _originalWeaponPos + new Vector3(0, -0.5f, -0.2f);
        float elapsed = 0f;
        float durationDown = 0.3f;
        float durationUp = 0.6f;

        while (elapsed < durationDown)
        {
            transform.localPosition = Vector3.Lerp(_originalWeaponPos, reloadPos, elapsed / durationDown);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (AudioManager._instance != null && _currentWeaponData._reloadSound != null)
        {
            AudioManager._instance.PlaySFX(_currentWeaponData._reloadSound);
        }

        float calculatedWait = _currentWeaponData._reloadTime - (durationDown + durationUp);
        if (calculatedWait > 0)
        {
            yield return new WaitForSeconds(calculatedWait);
        }

        elapsed = 0f;
        while (elapsed < durationUp)
        {
            transform.localPosition = Vector3.Lerp(reloadPos, _originalWeaponPos, elapsed / durationUp);
            elapsed += Time.deltaTime;
            yield return null;
        }

        int bulletsNeeded = _currentWeaponData._magSize - _bulletsInMag;
        int bulletsToAdd = Mathf.Min(bulletsNeeded, _ammoReserve);
        _bulletsInMag += bulletsToAdd;
        _ammoReserve -= bulletsToAdd;

        UpdateAmmoUI();
        _isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (_ammoText != null)
        {
            _ammoText.text = _bulletsInMag + " / " + _ammoReserve;
        }
    }
    public void RefillMaxAmmo()
    {
        if (_currentWeaponData != null)
        {
            _ammoReserve = _currentWeaponData._maxReserveAmmo;

            UpdateAmmoUI();
        }
    }
}