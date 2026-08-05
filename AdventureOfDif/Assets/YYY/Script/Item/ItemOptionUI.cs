using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemOptionUI : MonoBehaviour,
    ISelectHandler,
    IDeselectHandler
{
    public string itemKey;           // 例如 "ITEM_Potion"
    public Animator highlightAnim;   // 或 GameObject highlightObj
    public bool unlocked;            // 有无获得（数量>0 等同解锁）
    public int quantity;             // 数量
    public Text Quantity;            // 数量显示

    public ItemData data;             // ← 改成引用数据


    public void RefreshFromSave()
    {
        SaveData _data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);
        this.quantity = _data.GetItem(data.itemKey);

        unlocked = quantity > 0;
        gameObject.SetActive(unlocked);
        SetHighlight(false);
        // TODO: 刷新数量文字
        Quantity.text = quantity.ToString();
    }

    public void SetHighlight(bool on)
    {
        if (!highlightAnim) return;


        if (!highlightAnim.isActiveAndEnabled) return; // 物体未激活或组件被禁用
        if (highlightAnim.runtimeAnimatorController == null) return; // 没有控制器就别触发

        if (on && unlocked)
        {
            highlightAnim.SetTrigger("Pressed");
            // 把显示内容交给 UIManager
            UIManager.instance.ShowItemInfo(data, quantity);
        }
        else
        {
            highlightAnim.SetTrigger("Normal");
        }
    }

    public void UseItem()
    {
        if (!unlocked || quantity <= 0) return;

        // TODO: 实际消耗逻辑
        quantity--;
        SaveData _data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);
        _data.SetItem(data.itemKey, quantity);
        SaveManager.SaveGame(_data);

        Debug.Log($"使用物品：{itemKey} 剩余数量：{quantity}");


        switch(itemKey)
        {
            case "Pudding":
                RoomGenerator.instance.player.character.Heal(100);
                break;
            case "Cola":
                RoomGenerator.instance.player.character.Heal(200);
                break;
            case "ChocoBanana":
                RoomGenerator.instance.player.character.Heal(300);
                break;
            case "Cone":
                RoomGenerator.instance.player.character.Heal(500);
                break;
            case "CreamRoll":
                RoomGenerator.instance.player.character.Heal(700);
                break;
            case "FriedCutlet":
                RoomGenerator.instance.player.character.Heal(1000);
                break;
            case "Potion":
               // UIManager.instance.player.ChangeSex(100);
                break;
            case "Incense":
               // UIManager.instance.player.ChangeSex(200);
                break;
            case "ButtPlug":
               // UIManager.instance.player.ChangeSex(500);
                break;
            case "Vibrator":
               // UIManager.instance.player.ChangeSex(1000);
                break;
        }

        //刷新面板
        UIManager.instance.ShowItemInfo(data, quantity);

        // 数量用尽
        if (quantity <= 0)
        {
            unlocked = false;

            // 若“我”正是当前选中，先把选择挪走
            if (UIManager.instance.IsCurrentItem(this))
            {
                // 你希望向右/向下选谁就传什么步长（网格 3 列就传 +1 或 +3）
                UIManager.instance.MoveSelection_Item(+1); // 或 +3
            }

            // 再把自己隐藏
            gameObject.SetActive(false);
            return;
        }
        else 
        {
            //SetHighlight(true);//还是让自己显示
        }


        RefreshFromSave();//使用完物品刷新

    }


    public void OnSelect(BaseEventData eventData)
    {
        SetHighlight(true);
    }
    public void OnDeselect(BaseEventData eventData)
    {
        SetHighlight(false);
    }
    //public void OnSubmit(BaseEventData eventData)
    //{
    //    UseItem();
    //}
}
