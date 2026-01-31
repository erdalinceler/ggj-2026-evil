using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private const float DEFAULT_VOLUME = 1.0f;

    public static AudioManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private AudioDataSO audioData;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambientSource;

    // Runtime volume settings (not persisted to ScriptableObject)
    private float _currentMusicVolume = DEFAULT_VOLUME;
    private float _currentSFXVolume = DEFAULT_VOLUME;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        if (ambientSource == null)
            ambientSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        ambientSource.loop = true;
        sfxSource.playOnAwake = false;

        // Initialize runtime volume settings from ScriptableObject
        if (audioData != null)
        {
            _currentMusicVolume = audioData.musicVolume;
            _currentSFXVolume = audioData.sfxVolume;
        }

        UpdateVolumes();
    }

    public void UpdateVolumes()
    {
        if (musicSource != null)
            musicSource.volume = _currentMusicVolume;
        if (sfxSource != null)
            sfxSource.volume = _currentSFXVolume;
        if (ambientSource != null)
            ambientSource.volume = _currentSFXVolume;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null || ambientSource == null) return;

        if (ambientSource.clip == clip && ambientSource.isPlaying) return;

        ambientSource.clip = clip;
        ambientSource.Play();
    }

    public void StopAmbient()
    {
        if (ambientSource != null)
            ambientSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }



    public void SetMusicVolume(float volume)
    {
        _currentMusicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        _currentSFXVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
}
