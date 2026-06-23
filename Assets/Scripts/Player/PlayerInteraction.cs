using UnityEngine;
using TMPro;

namespace StarterAssets
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")]
        public float InteractionDistance = 3f;
        public Camera MainCamera;

        [Header("HUD UI")]
        [Tooltip("Text that appears when looking at something")]
        public TextMeshProUGUI InteractionPromptText;

        private StarterAssetsInputs _input;
        private PlayerScore _playerScore;

        private void Awake()
        {
            _input = GetComponent<StarterAssetsInputs>();
            _playerScore = GetComponent<PlayerScore>();
        }

        private void Update()
        {
            if (MainCamera == null || _input == null) return;

            Ray ray = MainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
            RaycastHit hit;

            bool lookingAtInteractable = false;

            if (Physics.Raycast(ray, out hit, InteractionDistance))
            {
                if (hit.collider.CompareTag("Bomb"))
                {
                    lookingAtInteractable = true;
                    if (InteractionPromptText != null)
                    {
                        InteractionPromptText.text = "Hold [E]";
                    }

                    if (_input.interact)
                    {
                        Bomb bomb = hit.collider.GetComponent<Bomb>();
                        if (bomb != null)
                        {
                            bomb.ReceiveDefuseInput();
                        }
                    }
                }
                else if (hit.collider.CompareTag("WallBuy"))
                {
                    MaxAmmo wallBuy = hit.collider.GetComponent<MaxAmmo>();
                    if (wallBuy != null)
                    {
                        lookingAtInteractable = true;

                        if (InteractionPromptText != null)
                        {
                            InteractionPromptText.text = wallBuy.GetPromptMessage();
                        }

                        if (_input.interact)
                        {
                            wallBuy.BuyMaxAmmo(_playerScore, gameObject);

                            _input.interact = false;
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