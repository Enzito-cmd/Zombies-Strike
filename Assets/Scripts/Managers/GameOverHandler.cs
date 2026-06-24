using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    public GameObject _panel;

    public void Show()
    {
        _panel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}