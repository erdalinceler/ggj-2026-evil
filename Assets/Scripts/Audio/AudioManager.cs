using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private AudioDataSO audioData;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambientSource;

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

        UpdateVolumes();
    }

    public void UpdateVolumes()
    {
        if (audioData == null) return;

        musicSource.volume = audioData.musicVolume;
        sfxSource.volume = audioData.sfxVolume;
        ambientSource.volume = audioData.sfxVolume;
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
        if (audioData != null)
        {
            audioData.musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (audioData != null)
        {
            audioData.sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
        }
    }
}
