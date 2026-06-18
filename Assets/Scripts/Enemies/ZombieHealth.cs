using UnityEngine;

namespace StarterAssets
{
    public class ZombieHealth : MonoBehaviour
    {
        public float MaxHealth = 100f;
        private float _currentHealth;

        private ZombieAI _zombieAI;
        private ZombieDissolve _zombieDissolve;

        private void Awake()
        {
            _zombieAI = GetComponent<ZombieAI>();
            _zombieDissolve = GetComponent<ZombieDissolve>();
        }

        private void OnEnable()
        {
            _currentHealth = MaxHealth;
        }

        public void TakeDamage(float damageAmount)
        {
            if (_currentHealth <= 0) return;

            _currentHealth -= damageAmount;

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Collider zombieCollider = GetComponent<Collider>();
            if (zombieCollider != null)
            {
                zombieCollider.enabled = false;
            }

            if (_zombieAI != null)
            {
                _zombieAI.TakeDamage();
            }

            if (_zombieDissolve != null)
            {
                _zombieDissolve.StartDissolve();
            }
            else
            {
                if (_zombieAI == null)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}