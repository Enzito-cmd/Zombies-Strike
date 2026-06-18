using UnityEngine;
using UnityEngine.Pool;

namespace StarterAssets
{
    public class ZombieSpawner : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Assign the Player Transform here. Zero hierarchy lookups!")]
        public Transform PlayerTransform;
        [Tooltip("The Zombie Prefab containing the ZombieAI component.")]
        public GameObject ZombiePrefab;
        [Tooltip("Array of Transforms representing spawn locations around the map.")]
        public Transform[] SpawnPoints;

        [Header("Spawn Pace")]
        public float TimeBetweenSpawns = 2f;

        [Header("Pool Limits")]
        public int DefaultPoolSize = 10;
        public int MaxPoolSize = 25; // Keep this value equal to or slightly higher than AbsoluteSimultaneousLimit

        private IObjectPool<GameObject> _zombiePool;
        private WaveManager _waveManager;
        private float _nextSpawnTime;

        private void Awake()
        {
            _waveManager = GetComponent<WaveManager>();

            // Initializing Unity's built-in Object Pool API
            _zombiePool = new ObjectPool<GameObject>(
                createFunc: OnCreateZombie,
                actionOnGet: OnGetZombie,
                actionOnRelease: OnReleaseZombie,
                actionOnDestroy: OnDestroyZombie,
                collectionCheck: false, // Disabled for a tiny extra performance boost
                defaultCapacity: DefaultPoolSize,
                maxSize: MaxPoolSize
            );
        }

        private void Update()
        {
            if (PlayerTransform == null || ZombiePrefab == null || SpawnPoints.Length == 0) return;

            // STRATEGY A OPTIMIZATION: Only advance the timer if WaveManager gives green light
            if (_waveManager.CanSpawnZombie())
            {
                if (Time.time >= _nextSpawnTime)
                {
                    _nextSpawnTime = Time.time + TimeBetweenSpawns;
                    _zombiePool.Get(); // Fetches an optimized zombie instance from the pool box
                }
            }
        }

        private GameObject OnCreateZombie()
        {
            GameObject zombie = Instantiate(ZombiePrefab);

            // We deactivate it IMMEDIATELY so it doesn't run its Update blindly
            zombie.SetActive(false);

            ZombieAI zombieScript = zombie.GetComponent<ZombieAI>();
            if (zombieScript != null)
            {
                // Inject dependencies before the zombie ever wakes up
                zombieScript.ConfigureZombieDependencies(PlayerTransform, _zombiePool, _waveManager);
            }

            return zombie;
        }

        // 2. GET: This puts the zombie on the field and turns it on safely
        private void OnGetZombie(GameObject zombie)
        {
            int randomIndex = Random.Range(0, SpawnPoints.Length);
            Transform selectedPoint = SpawnPoints[randomIndex];

            // Position it first while it's still sleeping
            zombie.transform.position = selectedPoint.position;
            zombie.transform.rotation = selectedPoint.rotation;

            // NOW we wake it up. It will run its logic with the Player already assigned!
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
            // Notify manager that a screen slot has been freed up
            _waveManager.RegisterZombieDeath();
        }

        private void OnDestroyZombie(GameObject zombie)
        {
            Destroy(zombie);
        }
    }
}