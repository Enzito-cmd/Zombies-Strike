using UnityEngine;
using TMPro;

namespace StarterAssets
{
    public class PlayerInteraction : MonoBehaviour
    {
        public float InteractionDistance = 3f;
        public Camera MainCamera;
        public TextMeshProUGUI InteractionPromptText;

        public bool IsVirtualButtonPressed;

        public void SetVirtualInteract(bool state)
        {
            IsVirtualButtonPressed = state;
        }

        private void Update()
        {
            if (MainCamera == null) return;

            Ray ray = MainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
            RaycastHit hit;
            bool lookingAtInteractable = false;

            if (Physics.Raycast(ray, out hit, InteractionDistance))
            {
                if (hit.collider.CompareTag("Bomb"))
                {
                    lookingAtInteractable = true;

                    if (IsVirtualButtonPressed)
                    {
                        Bomb bomb = hit.collider.GetComponent<Bomb>();
                        if (bomb != null) bomb.ReceiveDefuseInput();
                    }
                }
                else if (hit.collider.CompareTag("WallBuy"))
                {
                    lookingAtInteractable = true;

                    MaxAmmo ammoBuy = hit.collider.GetComponent<MaxAmmo>();
                    if (ammoBuy != null)
                    {
                        if (InteractionPromptText != null) InteractionPromptText.text = ammoBuy.GetPromptMessage();

                        if (IsVirtualButtonPressed)
                        {
                            ammoBuy.BuyMaxAmmo(GetComponent<PlayerScore>(), gameObject);
                            IsVirtualButtonPressed = false;
                        }
                    }

                    WallWeaponBuy weaponBuy = hit.collider.GetComponent<WallWeaponBuy>();
                    if (weaponBuy != null)
                    {
                        if (InteractionPromptText != null)
                        {
                            PlayerShooting playerShooting = gameObject.GetComponentInChildren<PlayerShooting>();
                            InteractionPromptText.text = weaponBuy.GetPromptMessage(playerShooting);
                        }

                        if (IsVirtualButtonPressed)
                        {
                            weaponBuy.Interact(gameObject);
                            IsVirtualButtonPressed = false;
                        }
                    }
                }
            }

            if (!lookingAtInteractable && InteractionPromptText != null)
            {
                InteractionPromptText.text = "";
            }
        }
    }
}