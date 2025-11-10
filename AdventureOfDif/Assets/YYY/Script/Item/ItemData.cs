using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 多语言文本
[System.Serializable]
public struct LocalizedText
{
    [TextArea] public string ja;   // 日
    [TextArea] public string zhCN; // 简中
    [TextArea] public string zhTW; // 繁中
    [TextArea] public string en;   // 英
    [TextArea] public string ko;   // 韩

    public string Get(string locale)
    {
        switch (locale)
        {
            case "ja": return string.IsNullOrEmpty(ja) ? en : ja;
            case "zhCN": return string.IsNullOrEmpty(zhCN) ? en : zhCN;
            case "zhTW": return string.IsNullOrEmpty(zhTW) ? en : zhTW;
            case "ko": return string.IsNullOrEmpty(ko) ? en : ko;
            default: return string.IsNullOrEmpty(en) ? zhCN : en;
        }
    }
}

public enum ItemEffectType
{
    HealInstant,      // 立刻回血 amount
    HealOverTime,     // 持续回血 amount, duration
    ArousalInstant,   // 立刻涨“性值”(ChangeSex) amount
    ArousalOverTime,  // 持续涨“性值” amount, duration
    BuffMoveSpeed,    // 移速加成 pct, duration
    Cleanse           // 驱散/清除异常
}

[CreateAssetMenu(fileName = "ItemData", menuName = "Game/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemKey;            // 如 "Pudding"
    public Sprite icon;

    public LocalizedText displayName; // 名称
    public LocalizedText description; // 简介

    // 效果
    public ItemEffectType effectType;
    public int amount = 0;            // 通用强度
    public float duration = 0f;       // 持续时间(秒)
    public float percent = 0f;        // 百分比效果（例如移速）
}