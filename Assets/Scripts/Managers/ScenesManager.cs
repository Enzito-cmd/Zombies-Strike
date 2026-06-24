using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance; 

    public GameObject _loadingScreen;
    public Slider _progressBar;
    public GameObject _gameOverPanel;

    private void Awake() 
    {
        Instance = this;
    }

    public void TriggerGameOver()
    {
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        LoadGame(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        LoadGame(0);
    }

    public void LoadGame(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        if (_loadingScreen != null) _loadingScreen.SetActive(true);

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            if (_progressBar != null)
                _progressBar.value = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}