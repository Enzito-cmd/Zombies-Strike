using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace StarterAssets
{
    public class Bomb : MonoBehaviour
    {
        [Header("Bomb Settings")]
        public float TimeToExplode = 60f;
        public float DefuseTimeRequired = 8f;

        [Header("UI References")]
        public GameObject BombUIPanel;
        public Image DefuseProgressBar; 
        public TextMeshProUGUI TimerText;

        private float _currentExplosionTimer;
        private float _currentDefuseProgress;
        private bool _isBombActive = false;
        private bool _isBeingDefusedThisFrame = false;

        public void ActivateBomb()
        {
            _currentExplosionTimer = TimeToExplode;
            _currentDefuseProgress = 0f;
            _isBombActive = true;

            if (BombUIPanel != null) BombUIPanel.SetActive(true);

            if (DefuseProgressBar != null)
            {
                DefuseProgressBar.fillAmount = 1f;
            }
        }

        private void Update()
        {
            if (!_isBombActive) return;

            _currentExplosionTimer -= Time.deltaTime;
            UpdateTimerUI();

            if (_currentExplosionTimer <= 0)
            {
                Explode();
            }

            if (_isBeingDefusedThisFrame)
            {
                _currentDefuseProgress += Time.deltaTime;

                if (DefuseProgressBar != null)
                {
                    float percentage = 1f - (_currentDefuseProgress / DefuseTimeRequired);
                    DefuseProgressBar.fillAmount = Mathf.Clamp01(percentage);
                }

                if (_currentDefuseProgress >= DefuseTimeRequired)
                {
                    SuccessfullyDefused();
                }
            }
            else
            {
                _currentDefuseProgress = 0f;
                if (DefuseProgressBar != null)
                {
                    DefuseProgressBar.fillAmount = 1f;
                }
            }

            _isBeingDefusedThisFrame = false;
        }

        public void ReceiveDefuseInput()
        {
            if (_isBombActive)
            {
                _isBeingDefusedThisFrame = true;
            }
        }

        private void UpdateTimerUI()
        {
            if (TimerText != null)
            {
                int minutes = Mathf.FloorToInt(_currentExplosionTimer / 60);
                int seconds = Mathf.FloorToInt(_currentExplosionTimer % 60);
                TimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }

        private void SuccessfullyDefused()
        {
            _isBombActive = false;
            if (BombUIPanel != null) BombUIPanel.SetActive(false);
            Destroy(gameObject, 1f);
        }

        private void Explode()
        {
            _isBombActive = false;
            if (BombUIPanel != null) BombUIPanel.SetActive(false);
            if (OffScreenIndicator.Instance != null)
            {
                OffScreenIndicator.Instance.SetTarget(null);
            }
            if (ScenesManager.Instance != null)
            {
                ScenesManager.Instance.TriggerGameOver();
            }
        }
    }
}