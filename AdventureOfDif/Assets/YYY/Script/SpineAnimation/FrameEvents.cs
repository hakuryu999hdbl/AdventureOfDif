using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameEvents : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<AudioManager>();
    }

    /// <summary>
    /// 声音
    /// </summary>
    #region
    [Header("声音")]
    public AudioManager AudioManager;
    public AudioSource audioS;

 

    //------------效果音
    //public void _BGM_Theme() { audioS.PlayOneShot(AudioManager.BGM_Theme); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用

    #endregion
}
