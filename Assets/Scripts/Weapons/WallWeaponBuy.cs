using UnityEngine;
using StarterAssets;
public class WallWeaponBuy : MonoBehaviour
{
    [Header("Weapon Settings")]
    public WeaponData _weaponToGive;
    public int _cost = 1000;

    public void Interact(GameObject playerObject)
    {
        PlayerShooting shooting = playerObject.GetComponentInChildren<PlayerShooting>();
        PlayerScore playerScore = playerObject.GetComponentInChildren<PlayerScore>();

        if (shooting != null && playerScore != null)
        {
            if (shooting.HasWeapon(_weaponToGive))
            {
                return;
            }

            if (playerScore.TrySpendPoints(_cost))
            {
                shooting.GiveWeapon(_weaponToGive);
            }
        }
    }

    public string GetPromptMessage(PlayerShooting playerShooting)
    {
        string weaponName = _weaponToGive != null ? _weaponToGive._weaponName : "Weapon";

        if (playerShooting != null && playerShooting.HasWeapon(_weaponToGive))
        {
            return $"M4 already bought";
        }

        return $"Press [Interact] to buy M4 [{_cost}]";
    }
}