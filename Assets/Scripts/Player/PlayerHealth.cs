using UnityEngine;
using Unity.Cinemachine; 

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int _maxHealth = 4;
    public int _currentHealth;

    [Header("Feedback References")]
    public BloodOverlay _bloodUI;

    private CinemachineImpulseSource _impulseSource;

    void Start()
    {
        _currentHealth = _maxHealth;
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damage;

        if (_bloodUI != null)
        {
            _bloodUI.ShowBlood();
        }

        if (_impulseSource != null)
        {
            _impulseSource.GenerateImpulse();
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("u died");
    }
}