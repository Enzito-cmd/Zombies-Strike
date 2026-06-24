using UnityEngine;
using Unity.Cinemachine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int _maxHealth = 4;
    public int _currentHealth;

    [Header("Regeneration Settings")]
    public float _regenDelay = 4f;      
    public float _timeBetweenHeals = 1f;
    private float _lastDamageTime;
    private float _nextHealTime;

    [Header("Feedback References")]
    public BloodOverlay _bloodUI;

    private CinemachineImpulseSource _impulseSource;

    void Start()
    {
        _currentHealth = _maxHealth;
        _impulseSource = GetComponent<CinemachineImpulseSource>();

        if (_bloodUI != null)
        {
            _bloodUI.UpdateBloodState(_currentHealth, _maxHealth);
        }
    }

    void Update()
    {
        HandleRegeneration();
    }

    public void TakeDamage(int damage)
    {
        if (_currentHealth <= 0) return;

        _currentHealth -= damage;
        _lastDamageTime = Time.time; 

        if (_bloodUI != null)
        {
            _bloodUI.ShowBloodFlash();
            _bloodUI.UpdateBloodState(_currentHealth, _maxHealth);
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

    private void HandleRegeneration()
    {
        if (_currentHealth < _maxHealth && Time.time > _lastDamageTime + _regenDelay)
        {
            if (Time.time >= _nextHealTime)
            {
                _currentHealth++;
                _nextHealTime = Time.time + _timeBetweenHeals;

                if (_bloodUI != null)
                {
                    _bloodUI.UpdateBloodState(_currentHealth, _maxHealth);
                }
            }
        }
    }

    void Die()
    {
        if (ScenesManager.Instance != null)
        {
            ScenesManager.Instance.TriggerGameOver();
        }
    }
}