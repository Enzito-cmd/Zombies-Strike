using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

namespace StarterAssets
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public class ZombieAI : MonoBehaviour
    {
        [Header("Zombie Settings")]
        public float AttackRange = 1.5f;
        public float Damage = 10f;
        public float AttackCooldown = 1.5f;

        [Header("Speed Odds")]
        [Range(0f, 1f)] public float SprintChance = 0.35f;
        public float WalkSpeed = 1.3f;
        public float SprintSpeed = 3.8f;

        private Transform _playerTransform;
        private NavMeshAgent _agent;
        private Animator _animator;
        private float _nextAttackTime;
        private bool _isDead = false;
        private float _attackRangeSqr;

        // Animation lock to prevent the zombie from sliding or chasing mid-attack state
        private bool _isAttackingAnimation = false;

        private IObjectPool<GameObject> _myPool;
        private WaveManager _waveManager;

        // Optimized Animator Hashes (Avoids costly string lookups during runtime)
        private static readonly int SpeedParam = Animator.StringToHash("Speed");
        private static readonly int AttackTrigger = Animator.StringToHash("Attack");
        private static readonly int DieParam = Animator.StringToHash("Die");

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _attackRangeSqr = AttackRange * AttackRange;
        }

        private void Start()
        {
            // Fallback: Autodetect player target if placed manually in the scene context
            if (_playerTransform == null)
            {
                GameObject playerObj = GameObject.FindWithTag("Player");
                if (playerObj != null)
                {
                    _playerTransform = playerObj.transform;
                }
                else
                {
                    var playerController = FindAnyObjectByType<FirstPersonController>();
                    if (playerController != null) _playerTransform = playerController.transform;
                }
            }

            if (_waveManager == null)
            {
                _waveManager = FindAnyObjectByType<WaveManager>();
            }

            DetermineZombieSpeed();
        }

        public void ConfigureZombieDependencies(Transform playerTarget, IObjectPool<GameObject> pool, WaveManager waveManager)
        {
            _playerTransform = playerTarget;
            _myPool = pool;
            _waveManager = waveManager;
        }

        public void ResetZombie()
        {
            _isDead = false;
            _isAttackingAnimation = false; // Ensures lock is cleared when reusing the object

            if (_agent != null)
            {
                _agent.enabled = true;
                _agent.Warp(transform.position); // Hard snap to the closest valid NavMesh position
                _agent.isStopped = false;
            }

            DetermineZombieSpeed();

            if (_playerTransform != null && _agent.enabled && _agent.isOnNavMesh)
            {
                _agent.SetDestination(_playerTransform.position);
            }
        }

        private void Update()
        {
            if (_isDead || _playerTransform == null) return;
            if (!_agent.isOnNavMesh) return;

            // Movement Lock: Completely stops tracking and pathfinding updates during the swipe animation
            if (_isAttackingAnimation) return;

            // Math Optimization: Squared magnitude vector subtraction to completely bypass costly square root ops
            Vector3 directionToPlayer = _playerTransform.position - transform.position;
            float sqrDistance = directionToPlayer.sqrMagnitude;

            if (sqrDistance <= _attackRangeSqr)
            {
                if (Time.time >= _nextAttackTime)
                {
                    _nextAttackTime = Time.time + AttackCooldown;

                    _isAttackingAnimation = true;
                    if (!_agent.isStopped) _agent.isStopped = true;

                    // Instantly zeroes out velocity to prevent inertia physics sliding ("skating effect")
                    _agent.velocity = Vector3.zero;

                    _animator.SetTrigger(AttackTrigger);
                    AttackPlayer();
                }
            }
            else
            {
                if (_agent.isStopped) _agent.isStopped = false;
                _agent.SetDestination(_playerTransform.position);
            }
        }

        private void DetermineZombieSpeed()
        {
            float actualSprintChance = SprintChance;

            // Dynamic BO2 Tuning: Automatically increases sprint odds based on the active progression wave
            if (_waveManager != null)
            {
                actualSprintChance = Mathf.Clamp(SprintChance + (_waveManager.CurrentWave * 0.05f), 0f, 0.9f);
            }

            if (Random.value <= actualSprintChance)
            {
                _agent.speed = SprintSpeed;
                _animator.SetFloat(SpeedParam, 2f);
            }
            else
            {
                _agent.speed = WalkSpeed;
                _animator.SetFloat(SpeedParam, 1f);
            }
        }

        private void AttackPlayer()
        {
            _nextAttackTime = Time.time + AttackCooldown;
        }

        // Triggered via Unity Animation Event framework on the exact frame of the hand impact strike
        public void DealDamage()
        {
            if (_isDead || _playerTransform == null) return;

            Vector3 directionToPlayer = _playerTransform.position - transform.position;
            float sqrDistance = directionToPlayer.sqrMagnitude;

            // Dodge Mechanic: Evaluates distance at hit frame
            if (sqrDistance <= _attackRangeSqr)
            {
                PlayerHealth playerHealth = _playerTransform.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)Damage);
                }
            }
        }

        // 2. NEW: Triggered via Animation Event at the VERY END of the attack animation
        public void EndAttack()
        {
            _isAttackingAnimation = false;
        }

        public void TakeDamage()
        {
            if (_isDead) return;
            _isDead = true;

            if (_agent != null) _agent.enabled = false;

            _animator.SetTrigger(DieParam);

            StartCoroutine(ReturnToPoolAfterDelay(3.5f));
        }

        private System.Collections.IEnumerator ReturnToPoolAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_myPool != null)
            {
                _myPool.Release(gameObject);
            }
            else
            {
                Destroy(gameObject); 
            }
        }
    }
}