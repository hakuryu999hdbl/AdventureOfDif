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




    public void _Attack_blood()
    {

        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Attack_blood1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Attack_blood2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.Attack_blood3);
                break;
        }
    }//这个由Player和Enemy中代码各自调用


    public void _Attack_sword_clash()
    {

        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Attack_sword_clash1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Attack_sword_clash2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.Attack_sword_clash3);
                break;
        }
    }//这个由Player和Enemy中代码各自调用



    public void _Attack_hit() 
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Attack_hit1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Attack_hit2);
                break;
        }
    }


    public void _Attack_pick() 
    {
        audioS.PlayOneShot(AudioManager.Attack_hit3);
    }//这个因为非常像抓取的声音


    public void _SE_falldown()
    {
        audioS.PlayOneShot(AudioManager.SE_falldown);
    }//这个由Player和Enemy中代码各自调用

    #endregion
}
