using UnityEngine;
using StarterAssets;
using System.Collections;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    [Header("References")]
    public Transform _firePoint;
    public StarterAssetsInputs _input;
    public TextMeshProUGUI _ammoText; 

    [Header("Settings")]
    public float _range = 100f;
    public float _fireRate = 0.2f;
    private float _nextTimeToFire = 0f;

    [Header("Ammo System")]
    public int _bulletsInMag = 12;
    public int _magSize = 12;
    public int _ammoReserve = 60;
    public float _reloadTime = 1.5f;
    private bool _isReloading = false;

    [Header("Visual Feedback")]
    public GameObject _muzzleFlashPrefab;
    public GameObject _impactEffectPrefab;
    public float _recoilAmount = 0.05f;

    [Header("Combat Settings")]
    public float _damage = 25f;  
    public GameObject _bloodImpactPrefab; 

    private Camera _mainCamera;
    private Vector3 _originalWeaponPos;

    void Start()
    {
        _mainCamera = Camera.main;
        _originalWeaponPos = transform.localPosition;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        UpdateAmmoUI(); 
    }

    void Update()
    {
        if (_input == null) return;
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

            if (_bulletsInMag < _magSize && _ammoReserve > 0)
            {
                StartCoroutine(Reload());
            }
        }

        if (_input.shoot && Time.time >= _nextTimeToFire && _bulletsInMag > 0)
        {
            _nextTimeToFire = Time.time + _fireRate;
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
        //feedback
        _bulletsInMag--;
        UpdateAmmoUI(); 

        if (AudioManager._instance != null)
        {
            AudioManager._instance.PlaySFX(AudioManager._instance._pistolShot);
        }

        if (_muzzleFlashPrefab != null)
        {
            GameObject _flash = Instantiate(_muzzleFlashPrefab, _firePoint.position, _firePoint.rotation);
            _flash.transform.SetParent(_firePoint);
            _flash.transform.localScale = Vector3.one;
            Destroy(_flash, 0.1f);
        }

        transform.localPosition -= Vector3.forward * _recoilAmount;

        //shooting logic
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _range))
        {
            ZombieHealth _zombie = hit.transform.GetComponent<ZombieHealth>();

            if (hit.transform.CompareTag("Enemy") && _zombie != null)
            {
                _zombie.TakeDamage(_damage);

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
        float durationDown = 0.3f; // Lo que tarda en bajar (rápido)
        float durationUp = 0.6f;   // Lo que tarda en subir (más lento, el doble)

        while (elapsed < durationDown)
        {
            transform.localPosition = Vector3.Lerp(_originalWeaponPos, reloadPos, elapsed / durationDown);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (AudioManager._instance != null && AudioManager._instance._pistolReload != null)
        {
            AudioManager._instance.PlaySFX(AudioManager._instance._pistolReload);
        }

        yield return new WaitForSeconds(1.3f);

        elapsed = 0f;
        while (elapsed < durationUp)
        {
            transform.localPosition = Vector3.Lerp(reloadPos, _originalWeaponPos, elapsed / durationUp);
            elapsed += Time.deltaTime;
            yield return null;
        }

        int bulletsNeeded = _magSize - _bulletsInMag;
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
    public void MaxAmmo()
    {
        _ammoReserve = 120; 
        _bulletsInMag = _magSize;
        UpdateAmmoUI();
    }
}