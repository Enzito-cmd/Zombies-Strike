using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("References")]
    private NavMeshAgent _agent;
    private Animator _animator;
    private Transform _player;
    private PlayerHealth _playerHealth;

    [Header("Settings")]
    public float _detectionRange = 15f;
    public float _attackRange = 1.5f;
    public float _attackRate = 1.5f;
    public int _damageAmount = 20;
    public float _attackStunDuration = 1f; 

    private float _nextAttackTime = 0f;
    private bool _isAttacking = false;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        if (_player != null)
        {
            _playerHealth = _player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (_player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer <= _attackRange)
        {
            if (Time.time >= _nextAttackTime)
            {
                Attack();
            }
        }
        else if (distanceToPlayer <= _detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            StopChasing();
        }

        if (_animator != null && _agent != null)
        {
            _animator.SetFloat("Speed", _agent.velocity.magnitude);
        }
    }

    void ChasePlayer()
    {
        if (_isAttacking) return;

        _agent.isStopped = false;
        _agent.SetDestination(_player.position);
    }

    void StopChasing()
    {
        _agent.isStopped = true;
    }

    void Attack()
    {
        _isAttacking = true;
        _nextAttackTime = Time.time + _attackRate;

        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }

        Invoke(nameof(ResumeMovement), _attackStunDuration);
    }

    public void DealDamage()
    {
        if (_player == null || _playerHealth == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceToPlayer <= _attackRange + 0.5f) 
        {
            _playerHealth.TakeDamage(_damageAmount);
        }
    }

    void ResumeMovement()
    {
        _isAttacking = false;
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.isStopped = false;
        }
    }
}