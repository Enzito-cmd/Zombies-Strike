using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class ScenesManager : MonoBehaviour
{
    public GameObject _loadingScreen;
    public Slider _progressBar;
    
    public void LoadGame(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    IEnumerator LoadAsync(int sceneIndex)
    {
        _loadingScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (_progressBar != null)
            {
                _progressBar.value = progress;
            }
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
