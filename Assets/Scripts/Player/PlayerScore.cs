using UnityEngine;
using TMPro;

namespace StarterAssets
{
    public class PlayerScore : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI ScoreTextUI;

        private int _currentScore = 0;

        private void Start()
        {
            UpdateScoreUI();
        }

        public void AddPoints(int pointsToAdd)
        {
            _currentScore += pointsToAdd;
            UpdateScoreUI();
        }

        public bool TrySpendPoints(int cost)
        {
            if (_currentScore >= cost)
            {
                _currentScore -= cost;
                UpdateScoreUI();
                return true; 
            }
            return false; 
        }

        private void UpdateScoreUI()
        {
            if (ScoreTextUI != null)
            {
                ScoreTextUI.text = _currentScore.ToString();
            }
        }
    }
}