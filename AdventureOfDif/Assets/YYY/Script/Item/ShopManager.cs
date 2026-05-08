using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEditor.MaterialProperty;
public class ShopManager : MonoBehaviour
{
    private void Start()
    {
        OpenShop(GameFlowData.ShopType);
        OnStart();
    }

    /// <summary>
    /// 商店系统
    /// </summary>
    #region
    public static ShopManager instance { get; private set; }
    void Awake()
    {
        instance = this;


    }

    public string currentLocale = "zhCN"; // "ja","zhCN","zhTW","en","ko"

    public void Getlanguage()
    {
        switch (PlayerPrefs.GetInt("language"))
        {
            case 0: currentLocale = "ja"; break;
            case 1: currentLocale = "zhCN"; break;
            case 2: currentLocale = "zhTW"; break;
            case 3: currentLocale = "en"; break;
            case 4: currentLocale = "ko"; break;

        }

    }

  




    [Header("商店类别")]
    public Image ShopImage;
    public Image Shop_Line;
    public Sprite Shop_Sex, Shop_Common_1, Shop_Common_2, Shop_Line_1, Shop_Line_2;


    public void OpenShop(int ShopType)//  1超市一  2超市二   3涩情超市
    {
        List<ItemData> chosenPool = null;
        switch (ShopType)
        {


            case 1:
                ShopImage.sprite = Shop_Common_1;
                Shop_Line.sprite = Shop_Line_1;
                chosenPool = shopPool; // 普通商店1
                break;

            case 2:
                ShopImage.sprite = Shop_Common_2;
                Shop_Line.sprite = Shop_Line_1;
                chosenPool = shopPool; // 普通商店2
                break;

            case 3:
                ShopImage.sprite = Shop_Sex;
                Shop_Line.sprite = Shop_Line_2;
                chosenPool = shopPool_Sex; // 特殊商品
                break;
        }


        

        OpenShopWithPool(chosenPool);
    }

    public void CloseShop()
    {
        SceneTransitionController transition = FindFirstObjectByType<SceneTransitionController>();

        if (transition != null)
        {
            transition.StartGame("YYY");
        }
        else
        {
            //为了直接打开YYY场景也可以跳转
            SceneManager.LoadScene("YYY", LoadSceneMode.Single);
        }

    }



    [Header("Shop")]
    public List<ShopItemUI> shopRows;     // 右侧4行
    public List<ItemData> shopPool;       // 全部可卖物品
    public List<ItemData> shopPool_Sex;   // 特殊可卖物品
    public Vector2 priceCoefRange = new Vector2(1.0f, 1.5f); // 不同店折扣/加价
    int shopIndex;


    public void OpenShopWithPool(List<ItemData> pool)
    {
        if (pool == null || pool.Count == 0)
        {
            Debug.LogWarning("⚠ 没有找到可用商品池！");
            return;
        }


        float coef = 1.0f;

        // 打乱并挑出前 4 个上架
        var shuffled = new List<ItemData>(pool);
        Shuffle(shuffled);



        for (int i = 0; i < shopRows.Count; i++)
        {
            if (i < pool.Count)
            {
                var d = pool[i];
                int price = Mathf.RoundToInt(d.Price * coef * UnityEngine.Random.Range(priceCoefRange.x, priceCoefRange.y));
                shopRows[i].Setup(d, price, -1, true); // 无限库存示例
            }
            else shopRows[i].Setup(null, 0, 0, false);
        }

        shopIndex = FindFirstActiveRow();
        UpdateHighlight_Shop();
    }

    int FindFirstActiveRow()
    {
        for (int i = 0; i < shopRows.Count; i++)
            if (shopRows[i].gameObject.activeInHierarchy) return i;
        return 0;
    }

    void UpdateHighlight_Shop()
    {
        for (int i = 0; i < shopRows.Count; i++)
            shopRows[i].SetHighlight(i == shopIndex);
    }

    public void MoveSelection_Shop(int step)
    {
        if (shopRows.Count == 0) return;
        shopRows[shopIndex].SetHighlight(false);
        int max = shopRows.Count;
        shopIndex = (shopIndex + step + max) % max;
        UpdateHighlight_Shop();
    }

    //public void ConfirmShop()
    //{
    //    if (shopRows.Count == 0) return;
    //    shopRows[shopIndex].Buy();
    //}

    // 工具
    void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    #endregion


    /// <summary>
    /// 菜单层面多端输入
    /// </summary>
    #region
    [SerializeField] private InputActionAsset inputActions;

    [Header("菜单层面多端输入")]
    private PlayerInputControl inputControl;
    public GameObject firstSelected;//打开手机第一个选中


    void OnStart()
    {
        inputControl = new PlayerInputControl();
        inputControl.UI.Cancel.started += OnCancel;


    }

    private void OnEnable()
    {


    }

    private void OnDisable()
    {

    }





  
  
    private void OnCancel(InputAction.CallbackContext ctx)
    {
 
        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);

    }

   





    #endregion

}
