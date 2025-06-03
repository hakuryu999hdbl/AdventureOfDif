using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{


    public static BGM instance { get; private set; }
    public AudioSource audioS;

    [Header("追逐音乐")]
    public List<AudioClip> ChaseMusicList; // 使用List来存储多个音乐
    public bool isPlaying;

    [Header("背景音乐")]
    public List<AudioClip> BackgroundMusicList;// 使用List来存储多个音乐

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        audioS = GetComponent<AudioSource>();
    }

    public void AudioPlayChaseMusic(int BGMNumber)
    {
        if (!isPlaying && ChaseMusicList.Count > 0)
        {

            if (BGMNumber < 0)
            {
                // 从列表中随机选择一首音乐
                audioS.clip = ChaseMusicList[Random.Range(0, ChaseMusicList.Count)];
            }
            else
            {
                audioS.clip = ChaseMusicList[BGMNumber];
            }//如果是小于0，那么随机播放，如果大于0，那么指定该序号播放
          

            //audioS.PlayOneShot(randomClip);

            // 将音频片段赋值给AudioSource的clip，并播放
            audioS.loop = true;  // 确保启用了循环播放
            audioS.Play();
            isPlaying = true;
        }

    }

    public void AudioPlayBackgroundMusic(int BGMNumber)
    {
        if (!isPlaying && BackgroundMusicList.Count > 0)
        {

            if (BGMNumber < 0)
            {
                // 从列表中随机选择一首音乐
                audioS.clip = BackgroundMusicList[Random.Range(0, BackgroundMusicList.Count)];
            }
            else
            {
                audioS.clip = BackgroundMusicList[BGMNumber];
            }//如果是小于0，那么随机播放，如果大于0，那么指定该序号播放


            // 将音频片段赋值给AudioSource的clip，并播放
            audioS.loop = true;  // 确保启用了循环播放
            audioS.Play();
            isPlaying = true;
        }

    }

    public void Stop()
    {
        audioS.Stop();
        isPlaying = false;
    }
}

