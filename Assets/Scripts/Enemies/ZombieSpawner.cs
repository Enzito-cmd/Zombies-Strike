using UnityEngine;
using UnityEngine.Pool;

namespace StarterAssets
{
    public class ZombieSpawner : MonoBehaviour
    {
        [Header("References")]
        public Transform PlayerTransform;
        public GameObject ZombiePrefab;
        public Transform[] SpawnPoints;

        [Header("Spawn Pace")]
        public float TimeBetweenSpawns = 2f;

        [Header("Pool Limits")]
        public int DefaultPoolSize = 10;
        public int MaxPoolSize = 25;

        private IObjectPool<GameObject> _zombiePool;
        private WaveManager _waveManager;
        private float _nextSpawnTime;

        private PlayerScore _cachedPlayerScore;

        private void Awake()
        {
            _waveManager = GetComponent<WaveManager>();

            _zombiePool = new ObjectPool<GameObject>(
                createFunc: OnCreateZombie,
                actionOnGet: OnGetZombie,
                actionOnRelease: OnReleaseZombie,
                actionOnDestroy: OnDestroyZombie,
                collectionCheck: false,
                defaultCapacity: DefaultPoolSize,
                maxSize: MaxPoolSize
            );
        }

        private void Start()
        {
            if (PlayerTransform != null)
            {
                _cachedPlayerScore = PlayerTransform.GetComponent<PlayerScore>();
            }
        }

        private void Update()
        {
            if (PlayerTransform == null || ZombiePrefab == null || SpawnPoints.Length == 0) return;

            if (_waveManager.CanSpawnZombie())
            {
                if (Time.time >= _nextSpawnTime)
                {
                    _nextSpawnTime = Time.time + TimeBetweenSpawns;
                    _zombiePool.Get();
                }
            }
        }

        private GameObject OnCreateZombie()
        {
            GameObject zombie = Instantiate(ZombiePrefab);
            zombie.SetActive(false);

            ZombieAI zombieScript = zombie.GetComponent<ZombieAI>();
            if (zombieScript != null)
            {
                zombieScript.ConfigureZombieDependencies(PlayerTransform, _zombiePool, _waveManager);
            }

            ZombieHealth healthScript = zombie.GetComponent<ZombieHealth>();
            if (healthScript != null)
            {
                healthScript.ConfigureHealthDependencies(_cachedPlayerScore);
            }

            return zombie;
        }

        private void OnGetZombie(GameObject zombie)
        {
            int randomIndex = Random.Range(0, SpawnPoints.Length);
            Transform selectedPoint = SpawnPoints[randomIndex];

            zombie.transform.position = selectedPoint.position;
            zombie.transform.rotation = selectedPoint.rotation;

            zombie.SetActive(true);

            ZombieAI zombieScript = zombie.GetComponent<ZombieAI>();
            if (zombieScript != null)
            {
                zombieScript.ResetZombie();
            }

            _waveManager.RegisterZombieSpawn();
        }

        private void OnReleaseZombie(GameObject zombie)
        {
            zombie.SetActive(false);
            _waveManager.RegisterZombieDeath();
        }

        private void OnDestroyZombie(GameObject zombie)
        {
            Destroy(zombie);
        }
    }
}