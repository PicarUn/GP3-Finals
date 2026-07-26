using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("Sound Data")]
    [Tooltip("Drag in a SoundLibrary asset. Create one via Assets > Create > Audio > Sound Library.")]
    [SerializeField] private SoundLibrary soundLibrary;

    [Header("SFX Pool")]
    [Tooltip("How many SFX can play at the same time.")]
    [SerializeField] private int sfxSourcePoolSize = 8;

    private AudioSource musicSource;
    private List<AudioSource> sfxPool = new List<AudioSource>();
    private Dictionary<string, Sound> musicLookup;
    private Dictionary<string, Sound> sfxLookup;

    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        BuildLookups();
        SetUpMusicSource();
        SetUpSfxPool();
    }

    private void BuildLookups()
    {
        musicLookup = new Dictionary<string, Sound>();
        sfxLookup = new Dictionary<string, Sound>();

        if (soundLibrary == null)
        {
            Debug.LogWarning("AudioManager: no SoundLibrary assigned. Assign one in the inspector.");
            return;
        }

        foreach (var s in soundLibrary.musicTracks)
        {
            if (s.clip == null || string.IsNullOrEmpty(s.name)) continue;
            musicLookup[s.name] = s;
        }

        foreach (var s in soundLibrary.sfxClips)
        {
            if (s.clip == null || string.IsNullOrEmpty(s.name)) continue;
            sfxLookup[s.name] = s;
        }
    }


    public void SetSoundLibrary(SoundLibrary newLibrary)
    {
        soundLibrary = newLibrary;
        BuildLookups();
    }

    private void SetUpMusicSource()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        if (musicMixerGroup != null)
            musicSource.outputAudioMixerGroup = musicMixerGroup;
    }

    private void SetUpSfxPool()
    {
        for (int i = 0; i < sfxSourcePoolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            if (sfxMixerGroup != null)
                src.outputAudioMixerGroup = sfxMixerGroup;
            sfxPool.Add(src);
        }
    }

    // ---------------- MUSIC ----------------

    public void PlayMusic(string trackName, bool restartIfSame = false)
    {
        if (!musicLookup.TryGetValue(trackName, out Sound sound))
        {
            Debug.LogWarning($"AudioManager: no music track named '{trackName}' found.");
            return;
        }

        if (musicSource.clip == sound.clip && musicSource.isPlaying && !restartIfSame)
            return;

        musicSource.clip = sound.clip;
        musicSource.volume = sound.volume;
        musicSource.pitch = sound.pitch;
        musicSource.loop = sound.loop || true; // music defaults to looping
        musicSource.Play();
    }

    public void PlayMusicFade(string trackName, float duration = 1f)
    {
        if (!musicLookup.TryGetValue(trackName, out Sound sound))
        {
            Debug.LogWarning($"AudioManager: no music track named '{trackName}' found.");
            return;
        }

        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeToTrack(sound, duration));
    }

    private IEnumerator FadeToTrack(Sound sound, float duration)
    {
        float startVolume = musicSource.volume;

        // Fade out
        float t = 0f;
        while (t < duration / 2f)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / (duration / 2f));
            yield return null;
        }

        musicSource.clip = sound.clip;
        musicSource.pitch = sound.pitch;
        musicSource.loop = true;
        musicSource.Play();

        // Fade in
        t = 0f;
        while (t < duration / 2f)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, sound.volume, t / (duration / 2f));
            yield return null;
        }

        musicSource.volume = sound.volume;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic() => musicSource.Pause();
    public void ResumeMusic() => musicSource.UnPause();

    // ---------------- SFX ----------------

    public void PlaySFX(string sfxName)
    {
        if (!sfxLookup.TryGetValue(sfxName, out Sound sound))
        {
            Debug.LogWarning($"AudioManager: no SFX named '{sfxName}' found.");
            return;
        }

        AudioSource src = GetAvailableSfxSource();
        src.clip = sound.clip;
        src.volume = sound.volume;
        src.pitch = sound.pitch;
        src.loop = sound.loop;
        src.Play();
    }

    public void PlaySFXWithPitchVariance(string sfxName, float variance = 0.1f)
    {
        if (!sfxLookup.TryGetValue(sfxName, out Sound sound))
        {
            Debug.LogWarning($"AudioManager: no SFX named '{sfxName}' found.");
            return;
        }

        AudioSource src = GetAvailableSfxSource();
        src.clip = sound.clip;
        src.volume = sound.volume;
        src.pitch = sound.pitch + UnityEngine.Random.Range(-variance, variance);
        src.loop = sound.loop;
        src.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        AudioSource src = GetAvailableSfxSource();
        src.clip = clip;
        src.volume = volume;
        src.pitch = pitch;
        src.loop = false;
        src.Play();
    }

    private AudioSource GetAvailableSfxSource()
    {
        foreach (var src in sfxPool)
        {
            if (!src.isPlaying)
                return src;
        }

        // Pool
        var newSrc = gameObject.AddComponent<AudioSource>();
        newSrc.playOnAwake = false;
        if (sfxMixerGroup != null)
            newSrc.outputAudioMixerGroup = sfxMixerGroup;
        sfxPool.Add(newSrc);
        return newSrc;
    }
}