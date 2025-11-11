using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class language : MonoBehaviour
{
    Text Text;


    public string J;
    public string C_1;
    public string C_2;
    public string E;
    public string K;

    public void OnEnable()
    {
        Text = GetComponent<Text>();

        string fullText = ""; // 存储要显示的完整文本
        // 根据保存的语言偏好设置文本
        switch (PlayerPrefs.GetInt("language"))
        {
            case 0:
                fullText = J;
                break;
            case 1:
                fullText = C_1;
                break;
            case 2:
                fullText = C_2;
                break;
            case 3:
                fullText = E;
                break;
            case 4:
                fullText = K;
                break;
            default:
                fullText = E;  // 默认英文
                break;
        }

        if (isTyped)
        {
            StartCoroutine(ShowText(fullText));  // 开始逐字显示文本
        }
        else
        {
            Text.text = fullText.ToString();//直接显示
        }

    }

    public bool isTyped = false;//是否需要这种打字效果
    float delay = 0.05f;   // 每个字的显示延迟时间

    IEnumerator ShowText(string textToShow)
    {
        Text.text = "";  // 先清空文本
        // 逐字显示
        for (int i = 0; i <= textToShow.Length; i++)
        {
            Text.text = textToShow.Substring(0, i);
            yield return new WaitForSeconds(delay);
        }
    }
}
