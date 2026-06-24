using UnityEngine;
using TMPro;

namespace StarterAssets
{
    public class WaveManager : MonoBehaviour
    {
        [Header("Main UI")]
        public TextMeshProUGUI WaveTextUI;

        [Header("Bomb Event")]
        public GameObject BombPrefab;
        public Transform[] BombSpawnPoints;

        [Header("Current Status")]
        public int CurrentWave = 1;
        public int TotalZombiesRemainingInWave;
        public int ActiveZombiesInScreen;

        [Header("Wave Balance")]
        public int BaseZombiesInFirstWave = 5;
        public int ZombiesPerWaveMultiplier = 2;

        [Header("Simultaneous Caps")]
        public int BaseMaxSimultaneousZombies = 4;
        public int MaxSimultaneousIncrementPerWave = 1;
        public int SimultaneousLimit = 24;

        private ZombieSpawner _spawner;
        private bool _isWaveActive = false;

        private void Start()
        {
            Application.targetFrameRate = 60;

            _spawner = GetComponent<ZombieSpawner>();
            StartNewWave();
        }

        private void StartNewWave()
        {
            _isWaveActive = true;
            ActiveZombiesInScreen = 0;

            TotalZombiesRemainingInWave = BaseZombiesInFirstWave + ((CurrentWave - 1) * ZombiesPerWaveMultiplier);

            UpdateUI();

            if (CurrentWave > 0 && CurrentWave % 3 == 0 && BombPrefab != null && BombSpawnPoints.Length > 0)
            {
                int randomPoint = Random.Range(0, BombSpawnPoints.Length);
                Transform spawnPoint = BombSpawnPoints[randomPoint];

                GameObject plantedBomb = Instantiate(BombPrefab, spawnPoint.position, spawnPoint.rotation);

                if (OffScreenIndicator.Instance != null)
                {
                    OffScreenIndicator.Instance.SetTarget(plantedBomb.transform);
                }

                Bomb bombScript = plantedBomb.GetComponent<Bomb>();
                if (bombScript != null)
                {
                    bombScript.ActivateBomb();
                }
            }
        }

        public bool CanSpawnZombie()
        {
            if (!_isWaveActive || TotalZombiesRemainingInWave <= 0) return false;

            int currentSimultaneousCap = BaseMaxSimultaneousZombies + ((CurrentWave - 1) * MaxSimultaneousIncrementPerWave);
            currentSimultaneousCap = Mathf.Min(currentSimultaneousCap, SimultaneousLimit);

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

        private void UpdateUI()
        {
            if (WaveTextUI != null)
            {
                WaveTextUI.text = CurrentWave.ToString();
            }
        }
    }
}