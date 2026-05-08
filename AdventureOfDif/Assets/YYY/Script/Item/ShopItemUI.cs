using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Pathfinding.RaycastModifier;

public class ShopItemUI : MonoBehaviour
{
    public ItemData data;
    public Animator highlightAnim;

    [Header("UI")]
    public Text priceText;       // 右侧价签
    public Text stockText;       // “已持有 x” 可选

    public Text Name;
    public Text Introduce;

    public Image Item_Image;

    [Header("Offer")]
    public int finalPrice;       // 本次上架价
    public int stock = -1;       // -1 为无限，>=0 为限量
    public bool active;          // 上架中

    public void Setup(ItemData d, int price, int stockCount = -1, bool isActive = true)
    {
        data = d; finalPrice = price; stock = stockCount; active = isActive;
        gameObject.SetActive(active && data != null);
        RefreshUI();
        SetHighlight(false);
    }

    public void RefreshUI()
    {
        if (!gameObject.activeInHierarchy || data == null) return;

        priceText.text = finalPrice.ToString();
        Name.text = data.displayName.Get(ShopManager.instance.currentLocale).ToString();
        Introduce.text = data.description.Get(ShopManager.instance.currentLocale).ToString();
        Item_Image.sprite = data.icon;

        //stockText.text = stock.ToString();//显示商店库存

        //显示玩家持有数量
        SaveData _data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);
        int owned = _data.GetItem(data.itemKey);



        stockText.text = owned.ToString();
        if (stock == 0) { gameObject.SetActive(false); }
    }

    public void SetHighlight(bool on)
    {
        if (!highlightAnim || highlightAnim.runtimeAnimatorController == null) return;
        bool can = active && data != null && (stock != 0);
        highlightAnim.SetTrigger(on && can ? "Pressed" : "Normal");

    }

    public void Buy()
    {
        if (!active || data == null || stock == 0) return;


        // 一次加载，贯穿整个购买流程
        SaveData _data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);

        // 钱不足拦截
        if (_data.Money < finalPrice)
        {
            GameFlowData.suppressNextClickSound = true;
            AudioManager.Instance.PlayFX(AudioManager.Instance.SE_falldown);
            return;
        }

        // ✅ 扣钱和加物品都在同一份 _data
        _data.Money -= finalPrice;
        _data.AddItem(data.itemKey, +1);

        // ✅ 一次保存
        SaveManager.SaveGame(_data);

        // ✅ UI 更新显示
        BalanceManager.instance.ChangeMoney(0, false);//更新钱
        RefreshUI();

        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);


        // 限购处理
        if (stock > 0) stock--;


    }
}
