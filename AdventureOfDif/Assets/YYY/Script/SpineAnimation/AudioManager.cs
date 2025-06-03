using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    public AudioSource audioS;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        audioS = GetComponent<AudioSource>();
    }

    public void AudioPlay(AudioClip clip)
    {
        if (audioS == null)
        {
            Debug.LogWarning("AudioSource is missing or destroyed.");
            return;  // 避免继续尝试播放已经销毁的音源
        }


        audioS.PlayOneShot(clip);
    }

    public void Stop()
    {
        audioS.Stop();
    }

    /// <summary>
    /// 声音
    /// </summary>
    #region
    [Header("效果音")]
    public AudioClip BGM_Theme;

    public AudioClip Attack_hit2;

    public AudioClip Bullet_Pistol, Bullet_Pistol_2, Bullet_Pistol_3, Bullet_Pistol_Load2,Bullet_SD, Bullet_SD_Load1, Bullet_AK, Bullet_OutOfBullet, SE_Blast;//这些Spine动画帧事件不再使用，FrameEvent那里作为脚本使用声音

    public AudioClip Attack_pai1, Attack_pai2, Attack_whip_1, Attack_whip_2, Attack_whip_3, Attack_whip_4, Attack_whip_5,
                     Attack_sword_chop1, Attack_sword_chop2, Attack_sword_chop3, Attack_katana, Attack_katana_in, Attack_katana_draw,
                     Attack_sword_clash2, Attack_sword_clash3,Attack_sword_clash4,
                     Attack_blood1, Attack_blood2, Attack_blood3,
                     Effect_tear1, Effect_falldown, Effect_tuo, Effect_zipper, SE_Clothes, Effect_camera, Effect_shower, Effect_water_buku_1, Effect_water_buku_2,
                     SE_Semen_1, SE_Semen_2, SE_Semen_3, SE_Semen_fuck_in, SE_Semen_fuck_out,
                     SE_Water, SE_Set, SE_Reba, SE_Swing, SE_Glass,
                     SE_Wings,

                     Jinye_zhajin1_fast, Jinye_zhajin1_middle, Jinye_zhajin1_one, Jinye_zhajin1_slow,
                     Jinye_yanxia, Jinv_xitian_fast1,

                     Jinye_tentacle_short, Jinye_tentacle_slow, Jinye_tentacle_middle, Jinye_tentacle_quick;

    public AudioClip SE_Rope,SE_Vibrator,SE_Electricity,magic_flame2;
    [Header("叶语嫣")]
    public AudioClip Audio_03_Breath_3_Short_2;
    public AudioClip Audio_03_Breath_0, Audio_03_Breath_1, Audio_03_Breath_2, Audio_03_Breath_3, Audio_03_Breath_4, Audio_03_Breath_5;
    public AudioClip YYY_WalkClip_1, YYY_WalkClip_2, YYY_RunClip_1, YYY_RunClip_2, FootClip;
    public AudioClip JK_attack1, JK_attack2, JK_attack3, JK_attack4;
    [Header("叶语嫣惨叫")]
    public AudioClip Audio_02_Syllabary_A;
    public AudioClip yyy_duzui1,yyy_duzui2, yyy_duzui3, yyy_duzui4, yyy_jianjiao4, yyy_jianjiao5,Rbq_niao_short1, Rbq_niao_short2, Rbq_niao_short3,
                     Audio_04_Scream_Strong_0, Audio_04_Scream_Strong_1, Audio_04_Scream_Strong_2, Audio_04_Scream_Strong_3, Audio_04_Scream_Strong_4, Audio_04_Scream_Strong_5, Audio_04_Scream_Strong_6, Audio_04_Scream_Strong_7;
    public AudioClip Audio_03_Voice_Struggle_1, Audio_03_Voice_Struggle_2, Audio_03_Voice_Strangle,
                     Audio_03_Resist_0, Audio_03_Resist_1, Audio_03_Resist_2, Audio_03_Resist_3, Audio_03_Resist_4, Audio_03_Resist_5,
                     Audio_03_Shame_0, Audio_03_Shame_1, Audio_03_Shame_2, Audio_03_Shame_3;
    public AudioClip Audio_04_Scream_Belly_0, Audio_04_Scream_Belly_1, Audio_04_Scream_Belly_2, Audio_04_Scream_Belly_3, Audio_04_Scream_Belly_4, Audio_04_Scream_Belly_5, Audio_04_Scream_Belly_6, Audio_04_Scream_Belly_7,
                     Audio_04_Scream_Face_0, Audio_04_Scream_Face_1, Audio_04_Scream_Face_2, Audio_04_Scream_Face_3, Audio_04_Scream_Face_4, Audio_04_Scream_Face_5,
                     Audio_04_Swoon_0, Audio_04_Swoon_1, Audio_04_Swoon_2, Audio_04_Swoon_3, Audio_04_Swoon_4,Audio_04_Swoon_5, Audio_04_Swoon_6;
    [Header("叶语嫣被强奸")]
    public AudioClip Audio_03_Cry_2_Short;
    public AudioClip Audio_03_Cry_0, Audio_03_Cry_1, Audio_03_Cry_2;

    public AudioClip Audio_01_Word_Aaaa, Audio_01_Word_Cannot, Audio_01_Word_ThankYou_1, Audio_01_Word_ThankYou_2;
    public AudioClip Audio_01_Word_ForgiveMe, Audio_01_Word_Hentai;
    public AudioClip Audio_01_Word_No_1, Audio_01_Word_No_2, Audio_01_Word_No_3, Audio_01_Word_No_4, Audio_01_Word_No_5, Audio_01_Word_No_6;

    public AudioClip Audio_01_Word_Stop_1, Audio_01_Word_Stop_2, Audio_01_Word_What, Audio_01_Word_This, Audio_01_Word_Wu;

    public AudioClip Audio_03_Fera_Semen_0, Audio_03_Fera_Semen_1, Audio_03_Fera_Swallow_0, Audio_03_Fera_0, Audio_03_Fera_1, Audio_03_Fera_2, Audio_03_Fera_3, Audio_03_Fera_4, Audio_03_Fera_5;

    public AudioClip Audio_03_H_Gasping_0, Audio_03_H_Gasping_1,
                     Audio_03_H_Gasping_Weak_0, Audio_03_H_Gasping_Weak_1,
                     Audio_03_H_Gasping_Quick_0, Audio_03_H_Gasping_Quick_1, Audio_03_H_Gasping_Quick_2,
                     Audio_03_03_H_ContinualClimax_0, Audio_03_03_H_ContinualClimax_1, Audio_03_03_H_ContinualClimax_2, Audio_03_03_H_ContinualClimax_3, Audio_03_03_H_ContinualClimax_4, Audio_03_03_H_ContinualClimax_5, Audio_03_03_H_ContinualClimax_6, Audio_03_03_H_ContinualClimax_7, Audio_03_03_H_ContinualClimax_8, Audio_03_03_H_ContinualClimax_9,
                     Audio_03_H_Gasping_MentalBreakDown_0, Audio_03_H_Gasping_MentalBreakDown_1,
                     Audio_03_H_Pain_0, Audio_03_H_Pain_1,
                     Audio_04_Scream_Weak_0, Audio_04_Scream_Weak_1, Audio_04_Scream_Weak_2,Audio_04_Scream_Weak_3,Audio_04_Scream_Weak_4,Audio_04_Scream_Weak_5;
    [Header("淫叫")]
    public AudioClip Audio_02_Connection_Gasping_Long_0;
    public AudioClip Audio_02_Connection_Gasping_0, Audio_02_Connection_Gasping_1, Audio_02_Connection_Gasping_2, Audio_02_Connection_Gasping_3, Audio_02_Connection_Gasping_4, Audio_02_Connection_Gasping_5, Audio_02_Connection_Gasping_6, Audio_02_Connection_Gasping_7,
                     Audio_02_Connection_Gasping_8, Audio_02_Connection_Gasping_9, Audio_02_Connection_Gasping_10, Audio_02_Connection_Gasping_11, Audio_02_Connection_Gasping_12, Audio_02_Connection_Gasping_13, Audio_02_Connection_Gasping_14, Audio_02_Connection_Gasping_15;
    public AudioClip Audio_02_Connection_Breather_0, Audio_02_Connection_Breather_1, Audio_02_Connection_Breather_2, Audio_02_Connection_Breather_3, Audio_02_Connection_Breather_4, Audio_02_Connection_Breather_5, Audio_02_Connection_Breather_6, Audio_02_Connection_Breather_7, Audio_02_Connection_Breather_8, Audio_02_Connection_Breather_9;

    [Header("男人")]
    public AudioClip Man_attack;
    public AudioClip Man_WalkClip_1, Man_WalkClip_2, Man_RunClip_1, Man_RunClip_2;
    public AudioClip Man_die1, Man_die2, Man_die3, Man_die4;
    [Header("怪物")]
    public AudioClip Shrike_Summon_Attack;
    public AudioClip Shrike_Die;
    public AudioClip monster_Summon_01, monster_Summon_02, monster_Attack_01, monster_Die_01;
    public AudioClip Zombie_Summon_1, Zombie_Summon_2, Zombie_Attack, Zombie_Die_1, Zombie_Die_2;
    public AudioClip Orangutan_Summon_1, Orangutan_Attack_1, Orangutan_Die_1;
    #endregion

    /// <summary>
    /// 转到外部网站
    /// </summary>
    #region
    public void OpenTwitter()
    {
        Application.OpenURL("https://x.com/Detective_ye");
    }
    public void OpenCi_en()
    {
        Application.OpenURL("https://ci-en.dlsite.com/creator/16247");
    }
    public void OpenPixiv()
    {
        Application.OpenURL("https://www.pixiv.net/users/38416908");
    }
    public void OpenDLsite()
    {
        Application.OpenURL("https://www.dlsite.com/maniax/work/=/product_id/RJ01296940.html");
    }
    public void OpenFanza()
    {
        Application.OpenURL("https://www.dmm.co.jp/dc/doujin/-/detail/=/cid=d_480255/?utm_source=twitter&utm_medium=social_tpost&utm_campaign=start&utm_term=d_480255&utm_content=doujin");
    }
    public void OpenSteam()
    {
        Application.OpenURL("https://store.steampowered.com/app/3297870/_/?beta=0");
    }
    #endregion


    /// <summary>
    /// 中途退出关闭App
    /// </summary>
    #region
    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("Menu", 0);
        Debug.Log("Application is quitting. Performing cleanup...");
        // 在这里执行一些清理操作
    }
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            PlayerPrefs.SetInt("Menu", 0);
            // 应用程序进入后台运行，执行相关操作
        }
        else
        {
            // 应用程序从后台切换到前台，执行相关操作
        }
    }
    #endregion
}
