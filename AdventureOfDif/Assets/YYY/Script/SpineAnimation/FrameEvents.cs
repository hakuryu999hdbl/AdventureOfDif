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
    public void _Attack_hit2() { audioS.PlayOneShot(AudioManager.Attack_hit2); }
    public void _Bullet_Pistol() { audioS.PlayOneShot(AudioManager.Bullet_Pistol); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用【目前还留着】
    public void _Bullet_Pistol_2() { audioS.PlayOneShot(AudioManager.Bullet_Pistol_2); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用【目前还留着】
    public void _Bullet_Pistol_3() { audioS.PlayOneShot(AudioManager.Bullet_Pistol_3); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用【目前还留着】
    public void _Bullet_Pistol_Load2() { audioS.PlayOneShot(AudioManager.Bullet_Pistol_Load2); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用【目前还留着】
    public void _Bullet_SD() { audioS.PlayOneShot(AudioManager.Bullet_SD); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用【目前还留着】
    public void _Bullet_SD_Load1() { audioS.PlayOneShot(AudioManager.Bullet_SD_Load1); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用【目前还留着】
    public void _Bullet_OutOfBullet() { audioS.PlayOneShot(AudioManager.Bullet_OutOfBullet); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Bullet_AK() { audioS.PlayOneShot(AudioManager.Bullet_AK); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用【目前还留着】
    public void _SE_Blast() { audioS.PlayOneShot(AudioManager.SE_Blast); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用

    public void _Attack_pai1() { audioS.PlayOneShot(AudioManager.Attack_pai1); }
    public void _Attack_pai2() { audioS.PlayOneShot(AudioManager.Attack_pai2); }
    public void _Attack_whip_1() { audioS.PlayOneShot(AudioManager.Attack_whip_1); }
    public void _Attack_whip_2() { audioS.PlayOneShot(AudioManager.Attack_whip_2); }
    public void _Attack_whip_3() { audioS.PlayOneShot(AudioManager.Attack_whip_3); }
    public void _Attack_whip_4() { audioS.PlayOneShot(AudioManager.Attack_whip_4); }
    public void _Attack_whip_5() { audioS.PlayOneShot(AudioManager.Attack_whip_5); }

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

    public void _Effect_falldown() { audioS.PlayOneShot(AudioManager.Effect_falldown); }
    public void _Effect_tuo() { audioS.PlayOneShot(AudioManager.Effect_tuo); }
    public void _Effect_zipper() { audioS.PlayOneShot(AudioManager.Effect_zipper); }
    public void _SE_Clothes() { audioS.PlayOneShot(AudioManager.SE_Clothes); }
    public void _Effect_camera() { audioS.PlayOneShot(AudioManager.Effect_camera); }
    public void _Effect_shower() { audioS.PlayOneShot(AudioManager.Effect_shower); }
    public void _Effect_water_buku_1() { audioS.PlayOneShot(AudioManager.Effect_water_buku_1); }
    public void _Effect_water_buku_2() { audioS.PlayOneShot(AudioManager.Effect_water_buku_2); }
    public void _Effect_tear1() { audioS.PlayOneShot(AudioManager.Effect_tear1); }
    public void _SE_Semen_1() { audioS.PlayOneShot(AudioManager.SE_Semen_1); }
    public void _SE_Semen_2() { audioS.PlayOneShot(AudioManager.SE_Semen_2); }
    public void _SE_Semen_3() { audioS.PlayOneShot(AudioManager.SE_Semen_3); }
    public void _SE_Semen_fuck_in() { audioS.PlayOneShot(AudioManager.SE_Semen_fuck_in); }
    public void _SE_Semen_fuck_out() { audioS.PlayOneShot(AudioManager.SE_Semen_fuck_out); }
    public void _SE_Water() { audioS.PlayOneShot(AudioManager.SE_Water); }
    public void _SE_Set() { audioS.PlayOneShot(AudioManager.SE_Set); }
    public void _SE_Reba() { audioS.PlayOneShot(AudioManager.SE_Reba); }
    public void _SE_Swing() { audioS.PlayOneShot(AudioManager.SE_Swing); }
    public void _SE_Glass() { audioS.PlayOneShot(AudioManager.SE_Glass); }
    public void _SE_Wings() { audioS.PlayOneShot(AudioManager.SE_Wings); }

    public void _Jinye_zhajin1_fast() { audioS.PlayOneShot(AudioManager.Jinye_zhajin1_fast); }
    public void _Jinye_zhajin1_middle() { audioS.PlayOneShot(AudioManager.Jinye_zhajin1_middle); }
    public void _Jinye_zhajin1_one() { audioS.PlayOneShot(AudioManager.Jinye_zhajin1_one); }
    public void _Jinye_zhajin1_slow() { audioS.PlayOneShot(AudioManager.Jinye_zhajin1_slow); }
    public void _Jinye_yanxia() { audioS.PlayOneShot(AudioManager.Jinye_yanxia); }
    public void _Jinv_xitian_fast1() { audioS.PlayOneShot(AudioManager.Jinv_xitian_fast1); }


    public void _Jinye_tentacle_short() { audioS.PlayOneShot(AudioManager.Jinye_tentacle_short); }

    public void _Jinye_tentacle_slow() { audioS.PlayOneShot(AudioManager.Jinye_tentacle_slow); }
    public void _Jinye_tentacle_middle() { audioS.PlayOneShot(AudioManager.Jinye_tentacle_middle); }
    public void _Jinye_tentacle_quick() { audioS.PlayOneShot(AudioManager.Jinye_tentacle_quick); }


    public void _SE_Rope() { audioS.PlayOneShot(AudioManager.SE_Rope); }
    public void _SE_Vibrator() { audioS.PlayOneShot(AudioManager.SE_Vibrator); }
    public void _SE_Electricity() { audioS.PlayOneShot(AudioManager.SE_Electricity); }

    public void _magic_flame2() { audioS.PlayOneShot(AudioManager.magic_flame2); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用


    //------------叶语嫣
    public void _SE_HighHeels_Walk_1() { audioS.PlayOneShot(AudioManager.YYY_WalkClip_1); }
    public void _SE_HighHeels_Walk_2() { audioS.PlayOneShot(AudioManager.YYY_WalkClip_2); }
    public void _SE_HighHeels_Run_1() { audioS.PlayOneShot(AudioManager.YYY_RunClip_1); }
    public void _SE_HighHeels_Run_2() { audioS.PlayOneShot(AudioManager.YYY_RunClip_2); }

    public void _JK_attack1() { audioS.PlayOneShot(AudioManager.JK_attack1); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用【目前还留着】
    public void _JK_attack2() { audioS.PlayOneShot(AudioManager.JK_attack2); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用【目前还留着】
    public void _JK_attack3() { audioS.PlayOneShot(AudioManager.JK_attack3); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用【目前还留着】
    public void _JK_attack4() { audioS.PlayOneShot(AudioManager.JK_attack4); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用【目前还留着】


    public void _03_Breath_3_Short_2() { audioS.PlayOneShot(AudioManager.Audio_03_Breath_3_Short_2); }

    public void _03_Breath_0() { audioS.PlayOneShot(AudioManager.Audio_03_Breath_0); }
    public void _03_Breath_1() { audioS.PlayOneShot(AudioManager.Audio_03_Breath_1); }
    public void _03_Breath_2() { audioS.PlayOneShot(AudioManager.Audio_03_Breath_2); }
    public void _03_Breath_3() { audioS.PlayOneShot(AudioManager.Audio_03_Breath_3); }
    public void _03_Breath_4() { audioS.PlayOneShot(AudioManager.Audio_03_Breath_4); }
    public void _03_Breath_5() { audioS.PlayOneShot(AudioManager.Audio_03_Breath_5); }
    public void _SE_Foot_1() { audioS.PlayOneShot(AudioManager.FootClip); }


    //------------叶语嫣惨叫
    public void _02_Syllabary_A() {  audioS.PlayOneShot(AudioManager.Audio_02_Syllabary_A); }
    public void _yyy_duzui1() {  audioS.PlayOneShot(AudioManager.yyy_duzui1); }
    public void _yyy_duzui2() { audioS.PlayOneShot(AudioManager.yyy_duzui2); }
    public void _yyy_duzui3() { audioS.PlayOneShot(AudioManager.yyy_duzui3); }
    public void _yyy_duzui4() {  audioS.PlayOneShot(AudioManager.yyy_duzui4); }
    public void _yyy_jianjiao4() { audioS.PlayOneShot(AudioManager.yyy_jianjiao4); }
    public void _yyy_jianjiao5() { audioS.PlayOneShot(AudioManager.yyy_jianjiao5); }
    public void _Rbq_niao_short1() { audioS.PlayOneShot(AudioManager.Rbq_niao_short1); }
    public void _Rbq_niao_short2() { audioS.PlayOneShot(AudioManager.Rbq_niao_short2); }
    public void _Rbq_niao_short3() { audioS.PlayOneShot(AudioManager.Rbq_niao_short3); }
    public void _04_Scream_Strong_0() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Strong_0); }
    public void _04_Scream_Strong_1() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Strong_1); }
    public void _04_Scream_Strong_2() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Strong_2); }
    public void _04_Scream_Strong_3() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Strong_3); }
    public void _04_Scream_Strong_4() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Strong_4); }
    public void _04_Scream_Strong_5() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Strong_5); }
    public void _04_Scream_Strong_6() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Strong_6); }
    public void _04_Scream_Strong_7() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Strong_7); }

    public void _03_Voice_Struggle_1() {  audioS.PlayOneShot(AudioManager.Audio_03_Voice_Struggle_1); }
    public void _03_Voice_Struggle_2() { audioS.PlayOneShot(AudioManager.Audio_03_Voice_Struggle_2); }
    public void _03_Voice_Strangle() { audioS.PlayOneShot(AudioManager.Audio_03_Voice_Strangle); }
    public void _03_Resist_0() {  audioS.PlayOneShot(AudioManager.Audio_03_Resist_0); }
    public void _03_Resist_1() {  audioS.PlayOneShot(AudioManager.Audio_03_Resist_1); }
    public void _03_Resist_2() {  audioS.PlayOneShot(AudioManager.Audio_03_Resist_2); }
    public void _03_Resist_3() {  audioS.PlayOneShot(AudioManager.Audio_03_Resist_3); }
    public void _03_Resist_4() {  audioS.PlayOneShot(AudioManager.Audio_03_Resist_4); }
    public void _03_Resist_5() {  audioS.PlayOneShot(AudioManager.Audio_03_Resist_5); }
    public void _03_Shame_0() {  audioS.PlayOneShot(AudioManager.Audio_03_Shame_0); }
    public void _03_Shame_1() {  audioS.PlayOneShot(AudioManager.Audio_03_Shame_1); }
    public void _03_Shame_2() {  audioS.PlayOneShot(AudioManager.Audio_03_Shame_2); }
    public void _03_Shame_3() {  audioS.PlayOneShot(AudioManager.Audio_03_Shame_3); }

    public void _04_Scream_Belly_0() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Belly_0); }
    public void _04_Scream_Belly_1() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Belly_1); }
    public void _04_Scream_Belly_2() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Belly_2); }
    public void _04_Scream_Belly_3() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Belly_3); }
    public void _04_Scream_Belly_4() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Belly_4); }
    public void _04_Scream_Belly_5() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Belly_5); }
    public void _04_Scream_Belly_6() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Belly_6); }
    public void _04_Scream_Belly_7() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Belly_7); }

    public void _04_Scream_Face_0() {  audioS.PlayOneShot(AudioManager.Audio_04_Scream_Face_0); }
    public void _04_Scream_Face_1() {  audioS.PlayOneShot(AudioManager.Audio_04_Scream_Face_1); }
    public void _04_Scream_Face_2() {  audioS.PlayOneShot(AudioManager.Audio_04_Scream_Face_2); }
    public void _04_Scream_Face_3() {  audioS.PlayOneShot(AudioManager.Audio_04_Scream_Face_3); }
    public void _04_Scream_Face_4() {  audioS.PlayOneShot(AudioManager.Audio_04_Scream_Face_4); }
    public void _04_Scream_Face_5() {  audioS.PlayOneShot(AudioManager.Audio_04_Scream_Face_5); }

    public void _04_Swoon_0() { audioS.PlayOneShot(AudioManager.Audio_04_Swoon_0); }
    public void _04_Swoon_1() { audioS.PlayOneShot(AudioManager.Audio_04_Swoon_1); }
    public void _04_Swoon_2() { audioS.PlayOneShot(AudioManager.Audio_04_Swoon_2); }
    public void _04_Swoon_3() { audioS.PlayOneShot(AudioManager.Audio_04_Swoon_3); }
    public void _04_Swoon_4() { audioS.PlayOneShot(AudioManager.Audio_04_Swoon_4); }
    public void _04_Swoon_5() { audioS.PlayOneShot(AudioManager.Audio_04_Swoon_5); }
    public void _04_Swoon_6() { audioS.PlayOneShot(AudioManager.Audio_04_Swoon_6); }



    //------------叶语嫣被强奸
    public void _03_Cry_2_Short() { audioS.PlayOneShot(AudioManager.Audio_03_Cry_2_Short); }
    public void _03_Cry_0() { audioS.PlayOneShot(AudioManager.Audio_03_Cry_0); }
    public void _03_Cry_1() { audioS.PlayOneShot(AudioManager.Audio_03_Cry_1); }
    public void _03_Cry_2() { audioS.PlayOneShot(AudioManager.Audio_03_Cry_2); }

    public void _01_Word_Aaaa() { audioS.PlayOneShot(AudioManager.Audio_01_Word_Aaaa); }
    public void _01_Word_Cannot() { audioS.PlayOneShot(AudioManager.Audio_01_Word_Cannot); }
    public void _01_Word_ThankYou_1() { audioS.PlayOneShot(AudioManager.Audio_01_Word_ThankYou_1); }
    public void _01_Word_ThankYou_2() { audioS.PlayOneShot(AudioManager.Audio_01_Word_ThankYou_2); }

    public void _01_Word_No_1() { audioS.PlayOneShot(AudioManager.Audio_01_Word_No_1); }
    public void _01_Word_No_2() { audioS.PlayOneShot(AudioManager.Audio_01_Word_No_2); }
    public void _01_Word_No_3() { audioS.PlayOneShot(AudioManager.Audio_01_Word_No_3); }
    public void _01_Word_No_4() { audioS.PlayOneShot(AudioManager.Audio_01_Word_No_4); }
    public void _01_Word_No_5() { audioS.PlayOneShot(AudioManager.Audio_01_Word_No_5); }
    public void _01_Word_No_6() { audioS.PlayOneShot(AudioManager.Audio_01_Word_No_6); }


    public void _01_Word_ForgiveMe() { audioS.PlayOneShot(AudioManager.Audio_01_Word_ForgiveMe); }
    public void _01_Word_Hentai() { audioS.PlayOneShot(AudioManager.Audio_01_Word_Hentai); }



    public void _01_Word_Stop_1() { audioS.PlayOneShot(AudioManager.Audio_01_Word_Stop_1); }
    public void _01_Word_Stop_2() { audioS.PlayOneShot(AudioManager.Audio_01_Word_Stop_2); }
    public void _01_Word_What() { audioS.PlayOneShot(AudioManager.Audio_01_Word_What); }
    public void _01_Word_This() { audioS.PlayOneShot(AudioManager.Audio_01_Word_This); }
    public void _01_Word_Wu() { audioS.PlayOneShot(AudioManager.Audio_01_Word_Wu); }

    public void _03_Fera_Semen_0() { audioS.PlayOneShot(AudioManager.Audio_03_Fera_Semen_0); }
    public void _03_Fera_Semen_1() { audioS.PlayOneShot(AudioManager.Audio_03_Fera_Semen_1); }
    public void _03_Fera_Swallow_0() { audioS.PlayOneShot(AudioManager.Audio_03_Fera_Swallow_0); }
    public void _03_Fera_0() { audioS.PlayOneShot(AudioManager.Audio_03_Fera_0); }
    public void _03_Fera_1() { audioS.PlayOneShot(AudioManager.Audio_03_Fera_1); }
    public void _03_Fera_2() { audioS.PlayOneShot(AudioManager.Audio_03_Fera_2); }
    public void _03_Fera_3() { audioS.PlayOneShot(AudioManager.Audio_03_Fera_3); }
    public void _03_Fera_4() { audioS.PlayOneShot(AudioManager.Audio_03_Fera_4);}
    public void _03_Fera_5() { audioS.PlayOneShot(AudioManager.Audio_03_Fera_5); }

    public void _03_H_Gasping_0() { audioS.PlayOneShot(AudioManager.Audio_03_H_Gasping_0); }
    public void _03_H_Gasping_1() { audioS.PlayOneShot(AudioManager.Audio_03_H_Gasping_1); }
    public void _03_H_Gasping_Weak_0() { audioS.PlayOneShot(AudioManager.Audio_03_H_Gasping_Weak_0); }
    public void _03_H_Gasping_Weak_1() { audioS.PlayOneShot(AudioManager.Audio_03_H_Gasping_Weak_1); }
    public void _03_H_Gasping_Quick_0() { audioS.PlayOneShot(AudioManager.Audio_03_H_Gasping_Quick_0); }
    public void _03_H_Gasping_Quick_1() { audioS.PlayOneShot(AudioManager.Audio_03_H_Gasping_Quick_1); }
    public void _03_H_Gasping_Quick_2() { audioS.PlayOneShot(AudioManager.Audio_03_H_Gasping_Quick_2); }
    public void _03_H_ContinualClimax_0() { audioS.PlayOneShot(AudioManager.Audio_03_03_H_ContinualClimax_0); }
    public void _03_H_ContinualClimax_1() { audioS.PlayOneShot(AudioManager.Audio_03_03_H_ContinualClimax_1); }
    public void _03_H_ContinualClimax_2() { audioS.PlayOneShot(AudioManager.Audio_03_03_H_ContinualClimax_2); }
    public void _03_H_ContinualClimax_3() { audioS.PlayOneShot(AudioManager.Audio_03_03_H_ContinualClimax_3); }
    public void _03_H_ContinualClimax_4() { audioS.PlayOneShot(AudioManager.Audio_03_03_H_ContinualClimax_4); }
    public void _03_H_ContinualClimax_5() { audioS.PlayOneShot(AudioManager.Audio_03_03_H_ContinualClimax_5); }
    public void _03_H_ContinualClimax_6() { audioS.PlayOneShot(AudioManager.Audio_03_03_H_ContinualClimax_6); }
    public void _03_H_ContinualClimax_7() { audioS.PlayOneShot(AudioManager.Audio_03_03_H_ContinualClimax_7); }
    public void _03_H_ContinualClimax_8() { audioS.PlayOneShot(AudioManager.Audio_03_03_H_ContinualClimax_8); }
    public void _03_H_ContinualClimax_9() { audioS.PlayOneShot(AudioManager.Audio_03_03_H_ContinualClimax_9); }

    public void _03_H_Gasping_MentalBreakDown_0() { audioS.PlayOneShot(AudioManager.Audio_03_H_Gasping_MentalBreakDown_0); }
    public void _03_H_Gasping_MentalBreakDown_1() { audioS.PlayOneShot(AudioManager.Audio_03_H_Gasping_MentalBreakDown_1); }


    public void _03_H_Pain_0() { audioS.PlayOneShot(AudioManager.Audio_03_H_Pain_0); }
    public void _03_H_Pain_1() { audioS.PlayOneShot(AudioManager.Audio_03_H_Pain_1); }
    public void _04_Scream_Weak_0() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Weak_0); }
    public void _04_Scream_Weak_1() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Weak_1); }
    public void _04_Scream_Weak_2() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Weak_2); }
    public void _04_Scream_Weak_3() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Weak_3); }
    public void _04_Scream_Weak_4() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Weak_4); }
    public void _04_Scream_Weak_5() { audioS.PlayOneShot(AudioManager.Audio_04_Scream_Weak_5); }
    //------------淫叫
    public void _02_Connection_Gasping_Long_0() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_Long_0); }
    public void _02_Connection_Gasping_0() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_0); }
    public void _02_Connection_Gasping_1() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_1); }
    public void _02_Connection_Gasping_2() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_2); }
    public void _02_Connection_Gasping_3() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_3); }
    public void _02_Connection_Gasping_4() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_4); }
    public void _02_Connection_Gasping_5() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_5); }
    public void _02_Connection_Gasping_6() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_6); }
    public void _02_Connection_Gasping_7() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_7); }
    public void _02_Connection_Gasping_8() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_8); }
    public void _02_Connection_Gasping_9() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_9); }
    public void _02_Connection_Gasping_10() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_10); }
    public void _02_Connection_Gasping_11() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_11); }
    public void _02_Connection_Gasping_12() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_12); }
    public void _02_Connection_Gasping_13() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_13); }
    public void _02_Connection_Gasping_14() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_14); }
    public void _02_Connection_Gasping_15() { audioS.PlayOneShot(AudioManager.Audio_02_Connection_Gasping_15); }


    public void _02_Connection_Breather_0() {  audioS.PlayOneShot(AudioManager.Audio_02_Connection_Breather_0); }
    public void _02_Connection_Breather_1() {  audioS.PlayOneShot(AudioManager.Audio_02_Connection_Breather_1); }
    public void _02_Connection_Breather_2() {  audioS.PlayOneShot(AudioManager.Audio_02_Connection_Breather_2); }
    public void _02_Connection_Breather_3() {  audioS.PlayOneShot(AudioManager.Audio_02_Connection_Breather_3); }
    public void _02_Connection_Breather_4() {  audioS.PlayOneShot(AudioManager.Audio_02_Connection_Breather_4); }
    public void _02_Connection_Breather_5() {  audioS.PlayOneShot(AudioManager.Audio_02_Connection_Breather_5); }
    public void _02_Connection_Breather_6() {  audioS.PlayOneShot(AudioManager.Audio_02_Connection_Breather_6); }
    public void _02_Connection_Breather_7() {  audioS.PlayOneShot(AudioManager.Audio_02_Connection_Breather_7); }
    public void _02_Connection_Breather_8() {  audioS.PlayOneShot(AudioManager.Audio_02_Connection_Breather_8); }
    public void _02_Connection_Breather_9() {  audioS.PlayOneShot(AudioManager.Audio_02_Connection_Breather_9); }
    //------------男人
    public void _SE_Shose_Walk_1() { audioS.PlayOneShot(AudioManager.Man_WalkClip_1); }
    public void _SE_Shose_Walk_2() { audioS.PlayOneShot(AudioManager.Man_WalkClip_2); }
    public void _SE_Shose_Run_1() { audioS.PlayOneShot(AudioManager.Man_RunClip_1); }
    public void _SE_Shose_Run_2() { audioS.PlayOneShot(AudioManager.Man_RunClip_2); }
    public void _Man_attack() { audioS.PlayOneShot(AudioManager.Man_attack); }
    public void _Man_die1() { audioS.PlayOneShot(AudioManager.Man_die1); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Man_die2() { audioS.PlayOneShot(AudioManager.Man_die2); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Man_die3() { audioS.PlayOneShot(AudioManager.Man_die3); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Man_die4() { audioS.PlayOneShot(AudioManager.Man_die4); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用



    //------------怪物
    public void _Shrike_Summon_Attack() { audioS.PlayOneShot(AudioManager.Shrike_Summon_Attack); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Shrike_Die() { audioS.PlayOneShot(AudioManager.Shrike_Die); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _monster_Summon_01() { audioS.PlayOneShot(AudioManager.monster_Summon_01); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _monster_Summon_02() { audioS.PlayOneShot(AudioManager.monster_Summon_02); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _monster_Attack_01() { audioS.PlayOneShot(AudioManager.monster_Attack_01); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _monster_Die_01() { audioS.PlayOneShot(AudioManager.monster_Die_01); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Zombie_Summon_1() { audioS.PlayOneShot(AudioManager.Zombie_Summon_1); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Zombie_Summon_2() { audioS.PlayOneShot(AudioManager.Zombie_Summon_2); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Zombie_Attack() { audioS.PlayOneShot(AudioManager.Zombie_Attack); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Zombie_Die_1() { audioS.PlayOneShot(AudioManager.Zombie_Die_1); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Zombie_Die_2() { audioS.PlayOneShot(AudioManager.Zombie_Die_2); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用

    public void _Orangutan_Summon_1() { audioS.PlayOneShot(AudioManager.Orangutan_Summon_1); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Orangutan_Attack_1() { audioS.PlayOneShot(AudioManager.Orangutan_Attack_1); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    public void _Orangutan_Die_1() { audioS.PlayOneShot(AudioManager.Orangutan_Die_1); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用

    #endregion
}
