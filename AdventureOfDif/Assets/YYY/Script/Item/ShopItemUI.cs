using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        Name.text = data.displayName.Get(UIManager.instance.currentLocale).ToString();
        Introduce.text = data.description.Get(UIManager.instance.currentLocale).ToString();
        Item_Image.sprite = data.icon;

        //stockText.text = stock.ToString();//显示商店库存

        //显示玩家持有数量
        int owned = PlayerPrefs.GetInt(data.itemKey, 0);
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

        int money = PlayerPrefs.GetInt("Money", 0);
        if (money < finalPrice)
        {
            AudioManager.instance.AudioPlay(AudioManager.instance.SE_Glass);
            return;
        }

        // 扣钱
        UIManager.instance.ChangeMoney(-finalPrice);

        // 加库存
        int have = PlayerPrefs.GetInt(data.itemKey, 0) + 1;
        PlayerPrefs.SetInt(data.itemKey, have);
        PlayerPrefs.Save();

        // 限购处理
        if (stock > 0) stock--;
        RefreshUI();

    }
}
