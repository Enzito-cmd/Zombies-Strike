using UnityEngine;

namespace StarterAssets
{
    public class ZombieHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        public float MaxHealth = 100f;
        private float _currentHealth;

        [Header("Rewards")]
        [SerializeField]
        private int _scoreReward = 50;

        private ZombieAI _zombieAI;
        private ZombieDissolve _zombieDissolve;
        private Collider _zombieCollider;

        private PlayerScore _playerScore; 

        private void Awake()
        {
            _zombieAI = GetComponent<ZombieAI>();
            _zombieDissolve = GetComponent<ZombieDissolve>();
            _zombieCollider = GetComponent<Collider>();
        }

        public void ConfigureHealthDependencies(PlayerScore playerScoreReference)
        {
            _playerScore = playerScoreReference;
        }

        private void OnEnable()
        {
            _currentHealth = MaxHealth;

            if (_zombieCollider != null)
            {
                _zombieCollider.enabled = true;
            }
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
            if (_zombieCollider != null)
            {
                _zombieCollider.enabled = false;
            }

            if (_playerScore != null)
            {
                _playerScore.AddPoints(_scoreReward);
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