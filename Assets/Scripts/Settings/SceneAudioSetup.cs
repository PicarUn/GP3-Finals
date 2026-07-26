using UnityEngine;

public class SceneAudioSetup : MonoBehaviour
{
    [SerializeField] private SoundLibrary sceneLibrary;

    [Header("scene start")]
    [SerializeField] private string startingTrack;
    [SerializeField] private bool fadeIn = true;
    [SerializeField] private float fadeDuration = 1f;

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("SceneAudioSetup: no AudioManager found in the scene hierarchy.");
            return;
        }

        AudioManager.Instance.SetSoundLibrary(sceneLibrary);

        if (string.IsNullOrEmpty(startingTrack)) return;

        if (fadeIn)
            AudioManager.Instance.PlayMusicFade(startingTrack, fadeDuration);
        else
            AudioManager.Instance.PlayMusic(startingTrack);
    }
}
