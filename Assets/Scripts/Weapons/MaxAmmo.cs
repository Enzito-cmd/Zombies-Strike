using UnityEngine;

namespace StarterAssets
{
    public class MaxAmmo : MonoBehaviour
    {
        [Header("Economy Settings")]
        [SerializeField] private int _ammoCost = 500;

        [Header("Cooldown")]
        private float _buyCooldown = 1f;
        private float _nextBuyTime;

        public string GetPromptMessage()
        {
            return $"Press [E] to buy ammo [{_ammoCost}]";
        }

        public void BuyMaxAmmo(PlayerScore playerScore, GameObject playerObject)
        {
            if (Time.time < _nextBuyTime) return;

            if (playerScore.TrySpendPoints(_ammoCost))
            {
                _nextBuyTime = Time.time + _buyCooldown;
                RefillPlayerAmmo(playerObject);
            }
        }

        private void RefillPlayerAmmo(GameObject playerObject)
        {
            PlayerShooting weaponSystem = playerObject.GetComponentInChildren<PlayerShooting>();

            if (weaponSystem != null)
            {
                weaponSystem.RefillMaxAmmo();
            }
        }
    }
}