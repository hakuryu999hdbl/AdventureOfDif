using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// 声音控制
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



    public static AudioManager Instance;

    private void Awake()
    {
        Instance = this;
    }



    public AudioSource bgmSource;
    public AudioSource fxSource;

    // 🎵 播放BGM（可循环）
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    // 🔊 播放音效（不打断）
    public void PlayFX(AudioClip clip)
    {
        if (clip == null) return;

        fxSource.PlayOneShot(clip);
    }//这个主要由UI层按键触发，局内通过FrameEvent自带的AuidoSoure播放范围音效）



    /// <summary>
    /// 声音
    /// </summary>
    #region
    [Header("效果音")]
    public AudioClip BGM_Theme;
    public AudioClip BGM_Level_1;

   

    public AudioClip UI_Click, UI_Select;

    public AudioClip SE_falldown;
    #endregion




}
