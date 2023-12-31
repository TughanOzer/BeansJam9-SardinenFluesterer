using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

/// <summary>
/// Allows you to 
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    #region Fields and Properties

    public static AudioManager Instance;

    [field: SerializeField] public AudioMixer MasterMixer { get; private set; }
    [field: SerializeField] public float MasterVolume { get; private set; } = 1;
    [field: SerializeField] public float MusicVolume { get; private set; } = 1;
    [field: SerializeField] public float SoundVolume { get; private set; } = 1;
    
    [SerializeField] private AudioSource _sfxVolumePreviewSound;
    [SerializeField] private AudioSource _buttonClick;

    #endregion

    #region Methods
     
    private void Awake()
    {
        

        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        } 
        Instance = this;
        DontDestroyOnLoad(this);

    }
    private void Start()
    {
        if(AudioManager.Instance != null)
        {
            //Sets start volumes if the should be adjusted immediately
            SetMasterVolume(AudioManager.Instance.MasterVolume);
            SetMusicVolume(AudioManager.Instance.MusicVolume);
            SetSoundVolume(AudioManager.Instance.SoundVolume, false);
        }
    }

    public void FadeInGameTrack(AudioSource audioSource)
    {
        if (!audioSource.isPlaying)
            audioSource.Play();

        audioSource.DOFade(1f, 3f);
    }

    public void FadeOutGameTrack(AudioSource audioSource, bool keepSilentlyPlaying = false)
    {
        if (!keepSilentlyPlaying)
            audioSource.DOFade(0, 3f).OnComplete(() => StopTrack(audioSource));
        else
            audioSource.DOFade(0, 3f);
    }

    private void StopTrack(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    //Plays a Sound Effect according to the enum index if it isn't playing already
    public void PlayOneShot(AudioSource audioSource)
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    //Plays a Sound Effect according to the enum index
    public void PlayUnlimited(AudioSource audioSource)
    {
        audioSource.Play();
    }

    public void PlayButtonClick()
    {
        Instance.PlayOneShot(_buttonClick);
    }

    public void SetMasterVolume(float volume)
    {
        var newVolume = GetLogCorrectedVolume(volume);
        MasterMixer.SetFloat("Master", newVolume);
        MasterVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        var newVolume = GetLogCorrectedVolume(volume);
        MasterMixer.SetFloat("Music", newVolume);
        MusicVolume = volume;
    }

    public void SetSoundVolume(float volume, bool changedBySlider = true)
    {
        var newVolume = GetLogCorrectedVolume(volume);
        MasterMixer.SetFloat("SFX", newVolume);
        SoundVolume = volume;

        //Play an exemplary SFX to give the play an auditory volume feedback
        if (changedBySlider)
            PlayOneShot(_sfxVolumePreviewSound);
    }

    private float GetLogCorrectedVolume(float volume)
    {
        return (volume > 0 ? Mathf.Log(volume) * 20f : -80f);
    }

    #endregion
}
