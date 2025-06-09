using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 血条等各种值
    /// </summary>
    #region
    [Header("生命值")]
    public Image HealthBar;

    [Header("淫乱值")]
    public Image SexBar;

    [Header("暴击值")]
    public Image CriticalBar;
    public GameObject R;

    // 用于闪烁控制
    private float flashTimer = 0f;
    private bool flashOn = false;

    public void UpdateHealthBar(int curAmount, int maxAmount)
    {
        HealthBar.fillAmount = (float)curAmount / (float)maxAmount;


        //if (curAmount <= maxAmount / 3)
        //{ HealthBar.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); }// 纯黄色
        //else if (curAmount > maxAmount / 3 && curAmount <= maxAmount / 2)
        //{ HealthBar.color = new Color(1.0f, 0.5f, 0.0f, 1.0f); } // 橙色
        //else
        //{ HealthBar.color = Color.red; }
    }

    public void UpdateSexBar(int curAmount, int maxAmount)
    {
        SexBar.fillAmount = (float)curAmount / (float)maxAmount;
    }


    public void UpdateCriticalBar(int curAmount, int maxAmount)
    {
        float fillPercent = (float)curAmount / (float)maxAmount;



        CriticalBar.fillAmount = fillPercent;

        // 多段颜色插值：蓝 → 绿 → 黄 → 红
        //Color baseColor;
        //
        //if (fillPercent < 0.33f) // 0%~33%：蓝到绿
        //{
        //    baseColor = Color.Lerp(new Color(0f, 0.5f, 1f), Color.green, fillPercent / 0.33f);
        //}
        //else if (fillPercent < 0.66f) // 33%~66%：绿到黄
        //{
        //    baseColor = Color.Lerp(Color.green, Color.yellow, (fillPercent - 0.33f) / 0.33f);
        //}
        //else // 66%~100%：黄到红
        //{
        //    baseColor = Color.Lerp(Color.yellow, Color.red, (fillPercent - 0.66f) / 0.34f);
        //}
        
        // 高暴击值闪烁（红黄闪）
        if (fillPercent > 0.9f)
        {
            //flashTimer += Time.deltaTime * 4f; // 闪烁速度
            //float alpha = Mathf.Abs(Mathf.Sin(flashTimer));
            //Color flashColor = Color.Lerp(baseColor, Color.blue, alpha); 
            //CriticalBar.color = flashColor;


            R.SetActive(true);
        }
        else
        {
            //CriticalBar.color = baseColor;

            R.SetActive(false);
        }
    }
    #endregion
}
