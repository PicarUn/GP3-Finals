using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "GameScene";
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;
    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private IEnumerator FadeAndLoadScene()
     {

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = 1f;
        SceneManager.LoadScene(gameSceneName);
    }
}