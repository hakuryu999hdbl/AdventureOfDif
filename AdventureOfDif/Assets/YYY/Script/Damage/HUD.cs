using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制伤害显示
/// </summary>
public class HUD : MonoBehaviour
{
    /// <summary>
    /// 滚动速度
    /// </summary>
    private float speed = 1.5f;

    /// <summary>
    /// 计时器
    /// </summary>
    private float timer = 0f;

    /// <summary>
    /// 销毁时间
    /// </summary>
    private float time = 1f;

    /// <summary>
    /// 玩家红 敌人白 回复绿
    /// </summary>
    public int color;

    private void Update()
    {
        Scroll();
    }

    /// <summary>
    /// 冒泡效果
    /// </summary>
    private void Scroll()
    {
        //字体滚动
        this.transform.Translate(Vector3.up * speed * Time.deltaTime);
        timer += Time.deltaTime;
        //字体缩小
        //this.GetComponent<Text>().fontSize--;
        //字体渐变透明

        switch (color)
        {
            case 0:
                this.GetComponent<Text>().color = new Color(1, 1, 1, 1 - timer);//白
                break;
            case 1:
                this.GetComponent<Text>().color = new Color(1, 0, 0, 1 - timer);//红
                break;
            case 2:
                this.GetComponent<Text>().color = new Color(0, 1, 0, 1 - timer);//绿
                break;
            case 3:
                this.GetComponent<Text>().color = new Color(1, 1, 0, 1 - timer);//黄
                break;
            case 4:
                this.GetComponent<Text>().color = new Color(0.4f, 0.8f, 1f, 1 - timer); // 更接近天蓝/亮青色
                break;




            case 5: 
                this.GetComponent<Text>().color = new Color(1, 0.6f, 0.2f, 1 - timer);// 橙
                break;
            case 6:
                this.GetComponent<Text>().color = new Color(0.8f, 0.4f, 1f, 1 - timer); // 紫
                break;

            case 7:
                this.GetComponent<Text>().color = new Color(1f, 0.4f, 0.7f, 1 - timer); // 粉红
                break;
            case 8:
                this.GetComponent<Text>().color = new Color(0.6f, 1f, 0.6f, 1 - timer); // 薄荷绿（限量感）
                break;
            case 9:
                this.GetComponent<Text>().color = new Color(1f, 1f, 0.5f, 1 - timer); // 柠檬黄
                break;
            case 10:
                this.GetComponent<Text>().color = new Color(1f, 0.2f, 0.2f, 1 - timer); // 猩红（偏限量红）
                break;
            case 11:
                this.GetComponent<Text>().color = new Color(0.5f, 0.8f, 0.2f, 1 - timer); // 青苹果绿
                break;
        }




        Destroy(gameObject, time);
    }

}