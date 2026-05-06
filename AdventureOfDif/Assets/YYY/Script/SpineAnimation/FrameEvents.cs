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
    public void _BGM_Theme() { audioS.PlayOneShot(AudioManager.BGM_Theme); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用

    public void _Attack_sword_chop1() { audioS.PlayOneShot(AudioManager.Attack_sword_chop1); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Attack_sword_chop2() { audioS.PlayOneShot(AudioManager.Attack_sword_chop2); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Attack_sword_chop3() { audioS.PlayOneShot(AudioManager.Attack_sword_chop3); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Attack_katana() { audioS.PlayOneShot(AudioManager.Attack_katana); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Attack_katana_in() { audioS.PlayOneShot(AudioManager.Attack_katana_in); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Attack_katana_draw() { audioS.PlayOneShot(AudioManager.Attack_katana_draw); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用

    public void _Attack_sword_clash2() { audioS.PlayOneShot(AudioManager.Attack_sword_clash2); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Attack_sword_clash3() { audioS.PlayOneShot(AudioManager.Attack_sword_clash3); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Attack_sword_clash4() { audioS.PlayOneShot(AudioManager.Attack_sword_clash4); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用

    public void _Attack_blood1() { audioS.PlayOneShot(AudioManager.Attack_blood1); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Attack_blood2() { audioS.PlayOneShot(AudioManager.Attack_blood2); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Attack_blood3() { audioS.PlayOneShot(AudioManager.Attack_blood3); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用

    #endregion
}
