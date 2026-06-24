using UnityEngine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class WeaponState
{
    public WeaponData data;
    public int bulletsInMag;
    public int ammoReserve;
}

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public StarterAssetsInputs _input;
    public TextMeshProUGUI _ammoText;
    public Image _weaponIconUI;
    public GameObject _impactEffectPrefab;
    public GameObject _bloodImpactPrefab;

    [Header("Inventory System")]
    public List<WeaponState> _inventory = new List<WeaponState>();
    private int _currentIndex = 0;

    [HideInInspector] public WeaponData _currentWeaponData;

    private int _bulletsInMag;
    private int _ammoReserve;
    private float _nextTimeToFire = 0f;
    private bool _isReloading = false;

    private Camera _mainCamera;
    private Vector3 _originalWeaponPos;
    private GameObject _currentWeaponModel;
    private Transform _dynamicFirePoint;
    private PlayerInput _playerInput;

    private GameObject _currentMuzzleFlashInstance;
    private Coroutine _flashCoroutine;

    public bool IsShootingButtonPressed;
    public void SetVirtualShoot(bool state) { IsShootingButtonPressed = state; }

    void Start()
    {
        _mainCamera = Camera.main;
        _originalWeaponPos = transform.localPosition;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        _playerInput = GetComponentInParent<PlayerInput>();

        if (_inventory.Count > 0 && _inventory[0].data != null)
        {
            EquipWeapon(0);
        }
    }

    public void GiveWeapon(WeaponData newWeapon)
    {
        for (int i = 0; i < _inventory.Count; i++)
        {
            if (_inventory[i].data == newWeapon)
            {
                _inventory[i].ammoReserve = newWeapon._maxReserveAmmo;
                if (_currentIndex == i)
                {
                    _ammoReserve = newWeapon._maxReserveAmmo;
                    UpdateAmmoUI();
                }
                return;
            }
        }

        WeaponState newState = new WeaponState();
        newState.data = newWeapon;
        newState.bulletsInMag = newWeapon._magSize;
        newState.ammoReserve = newWeapon._maxReserveAmmo;

        _inventory.Add(newState);
        EquipWeapon(_inventory.Count - 1);
    }

    public void CycleWeapon(int direction)
    {
        if (_inventory.Count <= 1 || _isReloading) return;

        int newIndex = _currentIndex + direction;
        if (newIndex >= _inventory.Count) newIndex = 0;
        if (newIndex < 0) newIndex = _inventory.Count - 1;

        EquipWeapon(newIndex);
    }

    private void EquipWeapon(int index)
    {
        if (_inventory.Count > 0 && _currentWeaponData != null && _currentWeaponModel != null)
        {
            _inventory[_currentIndex].bulletsInMag = _bulletsInMag;
            _inventory[_currentIndex].ammoReserve = _ammoReserve;
        }

        _currentIndex = index;
        WeaponState stateToEquip = _inventory[_currentIndex];

        _currentWeaponData = stateToEquip.data;
        _bulletsInMag = stateToEquip.bulletsInMag;
        _ammoReserve = stateToEquip.ammoReserve;

        if (_currentWeaponModel != null) Destroy(_currentWeaponModel);
        if (_currentMuzzleFlashInstance != null) Destroy(_currentMuzzleFlashInstance);

        if (_currentWeaponData._weaponModelPrefab != null)
        {
            _currentWeaponModel = Instantiate(_currentWeaponData._weaponModelPrefab, transform);
            _currentWeaponModel.transform.localPosition = Vector3.zero;
            _currentWeaponModel.transform.localRotation = Quaternion.identity;

            Transform foundFirePoint = _currentWeaponModel.transform.Find("FirePoint");
            _dynamicFirePoint = foundFirePoint != null ? foundFirePoint : transform;

            if (_currentWeaponData._muzzleFlashPrefab != null)
            {
                _currentMuzzleFlashInstance = Instantiate(_currentWeaponData._muzzleFlashPrefab, _dynamicFirePoint);
                _currentMuzzleFlashInstance.transform.localPosition = Vector3.zero;
                _currentMuzzleFlashInstance.transform.localRotation = Quaternion.identity;
                _currentMuzzleFlashInstance.SetActive(false);
            }
        }

        if (_weaponIconUI != null && _currentWeaponData._weaponIcon != null)
        {
            _weaponIconUI.sprite = _currentWeaponData._weaponIcon;
        }

        UpdateAmmoUI();
    }

    void Update()
    {
        if (_currentWeaponData == null) return;

        if (_isReloading)
        {
            if (_input != null) { _input.shoot = false; _input.reload = false; }
            return;
        }

        bool isHoldingShoot = (_input != null && _input.shoot) || IsShootingButtonPressed;

        if (_playerInput != null && _currentWeaponData._isAutomatic)
        {
            isHoldingShoot |= _playerInput.actions["Shoot"].IsPressed();
        }

        if (isHoldingShoot && _bulletsInMag <= 0 && _ammoReserve > 0)
        {
            if (_input != null) _input.shoot = false;
            IsShootingButtonPressed = false;
            StartCoroutine(Reload());
            return;
        }

        if (_input != null && _input.reload)
        {
            _input.reload = false;
            if (_bulletsInMag < _currentWeaponData._magSize && _ammoReserve > 0)
            {
                StartCoroutine(Reload());
            }
        }

        if (isHoldingShoot && Time.time >= _nextTimeToFire && _bulletsInMag > 0)
        {
            _nextTimeToFire = Time.time + _currentWeaponData._fireRate;
            Shoot();

            if (!_currentWeaponData._isAutomatic)
            {
                if (_input != null) _input.shoot = false;
                IsShootingButtonPressed = false;
            }
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

        if (_currentMuzzleFlashInstance != null)
        {
            if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
            _flashCoroutine = StartCoroutine(ShowMuzzleFlashSprite());
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
            else if (_impactEffectPrefab != null)
            {
                GameObject _impact = Instantiate(_impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(_impact, 1f);
            }
        }
    }

    private IEnumerator ShowMuzzleFlashSprite()
    {
        _currentMuzzleFlashInstance.SetActive(true);
        _currentMuzzleFlashInstance.transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        yield return new WaitForSeconds(0.04f);
        _currentMuzzleFlashInstance.SetActive(false);
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

        yield return new WaitForSeconds(Mathf.Max(0, _currentWeaponData._reloadTime - (durationDown + durationUp)));

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
        if (_ammoText != null) _ammoText.text = _bulletsInMag + " / " + _ammoReserve;
    }

    public void RefillMaxAmmo()
    {
        if (_currentWeaponData != null)
        {
            _ammoReserve = _currentWeaponData._maxReserveAmmo;
            UpdateAmmoUI();
        }
    }

    public bool HasWeapon(WeaponData weaponToCheck)
    {
        foreach (WeaponState slot in _inventory)
        {
            if (slot.data == weaponToCheck) return true;
        }
        return false;
    }
    public void VirtualReloadInput()
    {
        if (!_isReloading && _bulletsInMag < _currentWeaponData._magSize && _ammoReserve > 0)
        {
            StartCoroutine(Reload());
        }
    }
}