using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [Header("Settings")]
    public float _health = 100f;

    private bool _isDead = false;

    public void TakeDamage(float _amount)
    {
        if (_isDead) return;

        _health -= _amount;

        if (_health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        _isDead = true;

        Destroy(gameObject, 0.1f);
    }
}
