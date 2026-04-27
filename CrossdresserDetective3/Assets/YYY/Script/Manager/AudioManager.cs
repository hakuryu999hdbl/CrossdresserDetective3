using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{


    /// <summary>
    /// ê∫âπçTêß
    /// </summary>
    #region
    public AudioMixer Mixer;

    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider fxSlider;

    public void SetMasterVolume(float value)
    {
        Mixer.SetFloat("MasterVolume", value);
    }

    public void SetBGMVolume(float value)
    {
        Mixer.SetFloat("BGMVolume", value);
    }

    public void SetFXVolume(float value)
    {
        Mixer.SetFloat("FXVolume", value);
    }




    #endregion










    public AudioSource BGMSource;
    public AudioSource FXSource;
    public PlayAudioEventSO FXEvent;
    public PlayAudioEventSO BGMEvent;

    private void OnEnable()
    {
        FXEvent.OnEventRaised += OnFXEvent;
        BGMEvent.OnEventRaised += OnBGMEvent;
    }

    private void OnDisable()
    {
        FXEvent.OnEventRaised -= OnFXEvent;
        BGMEvent.OnEventRaised -= OnBGMEvent;
    }

    private void OnFXEvent(AudioClip clip)
    {
        FXSource.clip = clip;
        FXSource.Play();
    }

    private void OnBGMEvent(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();
    }
}
