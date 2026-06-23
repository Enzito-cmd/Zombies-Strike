using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Shooter/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Visual Info")]
    public string _weaponName;
    public GameObject _weaponModelPrefab; 
    public GameObject _muzzleFlashPrefab; 

    [Header("Stats")]
    public float _damage = 25f;
    public float _fireRate = 0.2f;
    public float _range = 100f;

    [Header("Ammo")]
    public int _magSize = 12;
    public int _maxReserveAmmo = 60;
    public float _reloadTime = 1.5f;

    [Header("Visual Feedback")]
    public float _recoilAmount = 0.05f;

    [Header("Audio")]
    public AudioClip _shootSound;
    public AudioClip _reloadSound;
}