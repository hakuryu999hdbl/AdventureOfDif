using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOptionUI : MonoBehaviour
{
    public string itemKey;           // 例如 "ITEM_Potion"
    public Animator highlightAnim;   // 或 GameObject highlightObj
    public bool unlocked;            // 有无获得（数量>0 等同解锁）
    public int quantity;             // 数量

    public void RefreshFromSave()
    {
        quantity = PlayerPrefs.GetInt(itemKey, 0);
        unlocked = quantity > 0;
        gameObject.SetActive(unlocked);
        SetHighlight(false);
        // TODO: 刷新数量文字
    }

    public void SetHighlight(bool on)
    {
        if (!highlightAnim) return;
        if (on && unlocked) highlightAnim.SetTrigger("Pressed");
        else highlightAnim.SetTrigger("Normal");
    }

    public void UseItem()
    {
        if (!unlocked || quantity <= 0) return;

        // TODO: 实际消耗逻辑
        quantity--;
        PlayerPrefs.SetInt(itemKey, quantity);

        if (quantity <= 0)
            gameObject.SetActive(false);

        Debug.Log("使用物品：" + itemKey);
    }
}
