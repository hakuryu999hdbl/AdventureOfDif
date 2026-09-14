using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class LanguageSelector : Selectable, IMoveHandler
{
    [Header("UI")]
    public Text textUI;                // 旧版 Text

    [Header("Languages")]
    public string[] languages =
    {
        "日本語",
        "简体中文",
        "繁體中文",
        "English",
        "한국어"
    };

    private int index;

    protected override void Start()
    {
        base.Start();

        index = PlayerPrefs.GetInt("language", 0); // 默认 日语
        Refresh();
    }

    public override void OnMove(AxisEventData eventData)
    {
        if (eventData.moveDir == MoveDirection.Left)
        {
            ChangeLanguage(-1);
        }
        else if (eventData.moveDir == MoveDirection.Right)
        {
            ChangeLanguage(1);
        }
        else
        {
            base.OnMove(eventData); // 上下交给导航
        }
        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
    }

    public void ChangeLanguage(int dir)
    {
        index += dir;

        if (index < 0)
            index = languages.Length - 1;
        else if (index >= languages.Length)
            index = 0;

        Refresh();

        PlayerPrefs.SetInt("language", index);
        PlayerPrefs.Save();

        foreach (var l in FindObjectsByType<language>(FindObjectsSortMode.None))
        {
            l.Refresh();
        }

    }

  

    private void Refresh()
    {
        

        string lang = languages[index];

        if (textUI != null)
            textUI.text = lang;
    }
}
