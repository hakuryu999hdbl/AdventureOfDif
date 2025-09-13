using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
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
    public bool isEnemy = false;//玩家受到伤害为{红-1}，敌人受到伤害为{白1}
    /// <summary>
    /// 生成伤害文字
    /// </summary>
    public void HUD(int damage)
    {

        //ShowNumber(damage);


        GameObject hud = Instantiate(hudText, transform) as GameObject;
       
       
        // 添加随机偏移（上下左右稍微错开）
        Vector3 offset = new Vector3(Random.Range(-15.5f, 15.5f), Random.Range(-12.3f, 12.3f), 0);
        hud.transform.localPosition += offset;
       
        hud.GetComponent<Text>().text = damage.ToString();
       
        if (damage < 0)
        {
            if (isEnemy)
            {
       
                hud.GetComponent<HUD>().color = 0;//敌人受伤为白
            }
            else
            {
                hud.GetComponent<HUD>().color = 1;//玩家受伤为红
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


    public Sprite[] numberSprites; // 0~9 的图片，按索引放好
    public GameObject digitPrefab; // 一个空的 Image 预制体（UI.Image组件）
    public float spacing = 20f;    // 数字间距

    private List<GameObject> digits = new List<GameObject>();

    public void ShowNumber(int value)
    {
        ClearDigits();

        string str = value.ToString();
        float startX = -(str.Length - 1) * spacing * 0.5f;

        for (int i = 0; i < str.Length; i++)
        {
            int num = str[i] - '0';

            GameObject go = Instantiate(digitPrefab, transform);
            go.GetComponent<Image>().sprite = numberSprites[num];

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(startX + i * spacing, 0);

            digits.Add(go);
        }
    }

    void ClearDigits()
    {
        foreach (var d in digits) Destroy(d);
        digits.Clear();
    }
}