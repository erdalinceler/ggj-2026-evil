using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioData")]
public class AudioDataSO : ScriptableObject
{
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;

    [Header("Music")]
    public AudioClip backgroundMusic;
    public AudioClip menuMusic;

    [Header("UI & Interface")]
    public AudioClip buttonClick;
    public AudioClip buttonHover;
    public AudioClip pageFlip;
    public AudioClip leverSwitch;
    public AudioClip documentRead;

    [Header("Weapon")]
    public AudioClip gunShot;

    [Header("Entity & Dialogue")]
    public AudioClip entityArrive;
    public AudioClip entityLeave;
    public AudioClip[] paperRustle;
    public AudioClip typewriterLoop;

    [Header("Feedback")]
    public AudioClip correctAnswer;
    public AudioClip wrongAnswer;
    public AudioClip scoreIncrease;
    public AudioClip scoreDecrease;

    [Header("Game States")]
    public AudioClip roundStart;
    public AudioClip roundEnd;
    public AudioClip victoryFanfare;
    public AudioClip defeatSound;
    public AudioClip perfectBonus;

    [Header("Ambient")]
    public AudioClip officeAmbience;
}
