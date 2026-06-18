using UnityEngine;

namespace StarterAssets
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Current Status")]
        public int CurrentWave = 1;
        public int TotalZombiesRemainingInWave;
        public int ActiveZombiesInScreen;

        [Header("Wave Balance")]
        [Tooltip("Total amount of zombies that will spawn during first wave")]
        public int BaseZombiesInFirstWave = 5;
        [Tooltip("How many extra zombies are added to the total pool per wave.")]
        public int ZombiesPerWaveMultiplier = 2;

        [Header("Simultaneous Caps")]
        [Tooltip("Maximum amount of zombies allowed alive at the exact same time during first wave")]
        public int BaseMaxSimultaneousZombies = 4;
        [Tooltip("How many extra simultaneous slots are unlocked per wave.")]
        public int MaxSimultaneousIncrementPerWave = 1;
        [Tooltip("The absolute simultaneous limit of active zombies")]
        public int AbsoluteSimultaneousLimit = 24;

        private ZombieSpawner _spawner;
        private bool _isWaveActive = false;

        private void Start()
        {
            _spawner = GetComponent<ZombieSpawner>();
            StartNewWave();
        }

        private void StartNewWave()
        {
            _isWaveActive = true;
            ActiveZombiesInScreen = 0;

            // Wave progression math (e.g., Wave 1 = 5, Wave 5 = 5 + (4 * 2) = 13 total zombies)
            TotalZombiesRemainingInWave = BaseZombiesInFirstWave + ((CurrentWave - 1) * ZombiesPerWaveMultiplier);

            Debug.Log($"Wave: {CurrentWave}. Horde size: {TotalZombiesRemainingInWave}");
        }

        // OPTIMIZATION: Fast status check used by the Spawner before fetching from the pool
        public bool CanSpawnZombie()
        {
            if (!_isWaveActive || TotalZombiesRemainingInWave <= 0) return false;

            // Calculate dynamic cap for the current wave
            int currentSimultaneousCap = BaseMaxSimultaneousZombies + ((CurrentWave - 1) * MaxSimultaneousIncrementPerWave);
            currentSimultaneousCap = Mathf.Min(currentSimultaneousCap, AbsoluteSimultaneousLimit);

            // Only allow spawn if we haven't reached the screen cap
            return ActiveZombiesInScreen < currentSimultaneousCap;
        }

        public void RegisterZombieSpawn()
        {
            ActiveZombiesInScreen++;
            TotalZombiesRemainingInWave--;
        }

        // Called when a zombie completely dies and returns to the pool
        public void RegisterZombieDeath()
        {
            ActiveZombiesInScreen--;

            // If no more zombies are left to spawn and none are active on screen, clear the wave
            if (TotalZombiesRemainingInWave <= 0 && ActiveZombiesInScreen <= 0)
            {
                EndWave();
            }
        }

        private void EndWave()
        {
            _isWaveActive = false;
            CurrentWave++;
            Debug.Log($"Wave finished. Next wave: {CurrentWave}...");

            Invoke(nameof(StartNewWave), 5f);
        }
    }
}