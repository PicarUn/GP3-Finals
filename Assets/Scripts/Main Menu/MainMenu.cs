using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public GameObject settingsPanel;
    public string gameSceneName = "Game";
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1.5f;

    private void Start()
    {
        settingsPanel.SetActive(false);
        AudioManager.Instance.PlayMusic("menuMusic");
        AudioManager.Instance.PlaySFX("humSFX");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void StartGame()
    {
        StartCoroutine(FadeAndLoadScene());
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