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

            TotalZombiesRemainingInWave = BaseZombiesInFirstWave + ((CurrentWave - 1) * ZombiesPerWaveMultiplier);

            Debug.Log($"Wave: {CurrentWave}. Horde size: {TotalZombiesRemainingInWave}");
        }

        public bool CanSpawnZombie()
        {
            if (!_isWaveActive || TotalZombiesRemainingInWave <= 0) return false;

            int currentSimultaneousCap = BaseMaxSimultaneousZombies + ((CurrentWave - 1) * MaxSimultaneousIncrementPerWave);
            currentSimultaneousCap = Mathf.Min(currentSimultaneousCap, AbsoluteSimultaneousLimit);

            return ActiveZombiesInScreen < currentSimultaneousCap;
        }

        public void RegisterZombieSpawn()
        {
            ActiveZombiesInScreen++;
            TotalZombiesRemainingInWave--;
        }

        public void RegisterZombieDeath()
        {
            ActiveZombiesInScreen--;

            if (TotalZombiesRemainingInWave <= 0 && ActiveZombiesInScreen <= 0)
            {
                EndWave();
            }
        }

        private void EndWave()
        {
            _isWaveActive = false;
            CurrentWave++;
            Invoke(nameof(StartNewWave), 5f);
        }
    }
}