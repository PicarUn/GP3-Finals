using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    [SerializeField] public AudioMixer Mixer;

    [Header("Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Other Sounds")]
    [SerializeField] private AudioClip buttonSFX;

    [Header("Scene Music Setup")]
    [SerializeField] private SceneMusicPair[] sceneMusicPairs;

    private Dictionary<string, AudioClip> sceneMusicDict;

    [System.Serializable]
    public class SceneMusicPair
    {
        public string sceneName;
        public AudioClip musicClip;
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        BuildMusicDictionary();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        LoadVolumes();
        PlaySceneMusic(SceneManager.GetActiveScene().name);
    }

    private void BuildMusicDictionary()
    {
        sceneMusicDict = new Dictionary<string, AudioClip>();

        foreach (var pair in sceneMusicPairs)
        {
            if (!sceneMusicDict.ContainsKey(pair.sceneName))
                sceneMusicDict.Add(pair.sceneName, pair.musicClip);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlaySceneMusic(scene.name);
    }

    private void PlaySceneMusic(string sceneName)
    {
        if (sceneMusicDict.TryGetValue(sceneName, out AudioClip clip))
        {
            ChangeBGM(clip);
        }
    }

    public void ChangeBGM(AudioClip newClip)
    {
        if (newClip == null) return;
        if (bgmSource.clip == newClip) return;

        bgmSource.clip = newClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    //SFXs
    public void PlayButtonSound()
    {
        PlaySFX(buttonSFX);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    //Volumes
    public void LoadVolumes()
    {
        SetMasterVolume(PlayerPrefs.GetFloat("masterVolume", 1f));
        SetMusicVolume(PlayerPrefs.GetFloat("musicVolume", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume", 1f));
    }

    public void SetMasterVolume(float value)
    {
        float v = Mathf.Clamp(value, 0.0001f, 1f);
        PlayerPrefs.SetFloat("masterVolume", v);
        Mixer.SetFloat("Master", Mathf.Log10(v) * 20);
    }

    public void SetMusicVolume(float value)
    {
        float v = Mathf.Clamp(value, 0.0001f, 1f);
        PlayerPrefs.SetFloat("musicVolume", v);
        Mixer.SetFloat("Music", Mathf.Log10(v) * 20);
    }

    public void SetSFXVolume(float value)
    {
        float v = Mathf.Clamp(value, 0.0001f, 1f);
        PlayerPrefs.SetFloat("sfxVolume", v);
        Mixer.SetFloat("SFX", Mathf.Log10(v) * 20);
    }
}
