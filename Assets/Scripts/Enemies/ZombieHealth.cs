using UnityEngine;

namespace StarterAssets
{
    public class ZombieHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        public float MaxHealth = 100f;
        private float _originalMaxHealth; 
        private float _currentHealth;

        [Header("Rewards")]
        [SerializeField]
        private int _scoreReward = 50;

        private ZombieAI _zombieAI;
        private Collider _zombieCollider;

        private PlayerScore _playerScore;

        private void Awake()
        {
            _zombieAI = GetComponent<ZombieAI>();
            _zombieCollider = GetComponent<Collider>();

            _originalMaxHealth = MaxHealth;
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

        public void ApplyDifficultyBonus(int currentRound)
        {
            int bonusLevel = Mathf.Min(currentRound / 5, 5);

            float bonusHealth = bonusLevel * 25f;

            MaxHealth = _originalMaxHealth + bonusHealth;
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