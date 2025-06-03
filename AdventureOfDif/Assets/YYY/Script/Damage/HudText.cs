using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制伤害效果的生成，附在Canvas上
/// </summary>
public class HudText : MonoBehaviour
{
    /// <summary>
    /// 文字预制体
    /// </summary>
    public GameObject hudText;
    public bool isFriend = false;//玩家和队友受到伤害为{红-1}，敌人受到伤害为{白1}
    /// <summary>
    /// 生成伤害文字
    /// </summary>
    public void HUD(int damage)
    {
        GameObject hud = Instantiate(hudText, transform) as GameObject;


        // 添加随机偏移（上下左右稍微错开）
        Vector3 offset = new Vector3(Random.Range(-15.5f, 15.5f), Random.Range(-12.3f, 12.3f), 0);
        hud.transform.localPosition += offset;

        hud.GetComponent<Text>().text = damage.ToString();

        if (damage < 0)
        {
            if (!isFriend)
            {

                hud.GetComponent<HUD>().color = 0;//敌人受伤为白
            }
            else
            {
                hud.GetComponent<HUD>().color = 1;//玩家和队友受伤为红
            }
        }
        else
        {
            hud.GetComponent<HUD>().color = 2;//双方回血都是绿
        }

        if(damage == 0)
        {
            hud.GetComponent<Text>().text = "Miss!".ToString();
            hud.GetComponent<HUD>().color = 3;//双方闪避都是黄
        }

    }

    public void SpecialText(int TextNumber) 
    {

        GameObject hud = Instantiate(hudText, transform) as GameObject;

        switch (TextNumber)
        {
            case 0:
                hud.GetComponent<Text>().text = "No Mana!".ToString();
                hud.GetComponent<HUD>().color = 4;//体力不足是蓝
                break;
            case 1:
                hud.GetComponent<Text>().text = "Dodge!".ToString(); 
                hud.GetComponent<HUD>().color = 3;//闪避成功是黄
                break;
            case 2:
                hud.GetComponent<Text>().text = "Critical!".ToString(); 
                hud.GetComponent<HUD>().color = 10;//暴击成功是猩红
                break;
        }

    }

}