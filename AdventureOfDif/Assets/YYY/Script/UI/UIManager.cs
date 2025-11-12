using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework.Interfaces;
using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Pathfinding.RaycastModifier;


public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
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

    [Header("挣扎值")]
    public Image StruggleBar;
    public GameObject Struggle;

    // 用于闪烁控制
    //private float flashTimer = 0f;
    //private bool flashOn = false;

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
        float percent = Mathf.Clamp01((float)curAmount / (float)maxAmount);

        // 确保使用的是材质实例（不共享全局材质）
        if (SexBar.material != null)
        {
            SexBar.material.SetFloat("_SexValue", percent);
        }
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


    public void UpdateStruggleBar(int curAmount, int maxAmount)
    {
        StruggleBar.fillAmount = (float)curAmount / (float)maxAmount;

        if (curAmount <= 0)
        {

            Struggle.SetActive(false);

        }
        else
        {

            Struggle.SetActive(true);

        }
    }

    #endregion



    /// <summary>
    /// 区域背景音乐（Start在这里）
    /// </summary>
    #region

    public void Start()
    {
        PlayRegionBGM(1);

        UpdatePhonePage_Highlight(); // 初始化【手机主界面】高亮

        ItemUnclockStart();//初始化【菜单界面】高亮

        //PlayerPrefs.SetInt("Cola",12);
        //PlayerPrefs.SetInt("ChocoBanana",4);
        //PlayerPrefs.SetInt("Pudding", 3);
        //PlayerPrefs.SetInt("Potion", 6);

        //初始无物品显示
        itemIcon.gameObject.SetActive(false);
        itemNameText.gameObject.SetActive(false);
        itemDescText.gameObject.SetActive(false);


        ChangeMoney(0, false);//更新钱

    }
    public void PlayRegionBGM(int ChangeBGM)
    {
        if (ChangeBGM == 0)
        {
            BGM.instance.AudioPlayMenuMusic(-1);//播放主菜单背景音乐
        }
        else
        {
            BGM.instance.AudioPlayBackgroundMusic(-1);//播放场景内背景音乐
        }


    }

    #endregion


    /// <summary>
    /// BGM/SE设置
    /// </summary>
    #region  

    [SerializeField] private GameObject[] SettingPage_highlightObjs; // 设置高亮显示
    public int SettingPagecurrentIndex = 0;//0 BGM  1 SE
    private void UpdateSettingPage_Highlight()
    {
        for (int i = 0; i < SettingPage_highlightObjs.Length; i++)
        {
            SettingPage_highlightObjs[i].SetActive(i == SettingPagecurrentIndex);
        }
    }










    [Header("BGM/SE设置")]
    public AudioMixer audioMixer;
    public AudioMixer BGM_Mixer;



    public Image BGM_Bar;
    public Image SE_Bar;

    public float BGMVolume = 0f;
    public float SEVolume = 0f;

    private const float MinVolume = -80f;
    private const float MaxVolume = 0f;


    //-------- SE --------
    public void SetSEVolune(float value)
    {
        SEVolume = Mathf.Clamp(value, MinVolume, MaxVolume);
        audioMixer.SetFloat("MainVolume", SEVolume);
        SE_Bar.fillAmount = Mathf.InverseLerp(MinVolume, MaxVolume, SEVolume);
    }

    public void SE_Up()
    {
        SetSEVolune(SEVolume + 10f);
        Debug.Log("拉高 SE 音量：" + SEVolume);
    }

    public void SE_Down()
    {
        SetSEVolune(SEVolume - 10f);
        Debug.Log("降低 SE 音量：" + SEVolume);
    }

    //-------- BGM --------
    public void SetBGMVolune(float value)
    {
        BGMVolume = Mathf.Clamp(value, MinVolume, MaxVolume);
        BGM_Mixer.SetFloat("BGMVolume", BGMVolume);
        BGM_Bar.fillAmount = Mathf.InverseLerp(MinVolume, MaxVolume, BGMVolume);
    }

    public void BGM_Up()
    {
        SetBGMVolune(BGMVolume + 10f);
        Debug.Log("拉高 BGM 音量：" + BGMVolume);
    }

    public void BGM_Down()
    {
        SetBGMVolune(BGMVolume - 10f);
        Debug.Log("降低 BGM 音量：" + BGMVolume);
    }

    #endregion

    /// <summary>
    /// 手机菜单系统
    /// </summary>
    #region
    [Header("手机菜单系统")]
    public Animator Anim_Phone;
    public Player player;

    public void OpenPhone()
    {


        //处于AVG和商店的状况下，无法打开手机
        if (CurrentChooseMenu == -1 || CurrentChooseMenu == -2) 
        {
            return;
        }


        Anim_Phone.SetBool("Open", true);

        //Time.timeScale = 0;

        CurrentChooseMenu = 1;
        player.isInputBlocked = true;



        //打开手机敌人不能动
        //StopAllEnemy();
    }
    public void ClosePhone()
    {
        Anim_Phone.SetBool("Open", false);

        //Time.timeScale = 1;

        CurrentChooseMenu = 0;
        player.isInputBlocked = false;

        //关掉手机敌人可以动
        //AllowAllEnemy();
    }





    public GameObject ItemPage;
    public void Open_ItemMenu()
    {
        ItemPage.SetActive(true);
        CurrentChooseMenu = 2;

        UpdateHighlight_Item();

        ItemUnclockStart();//初始化【菜单界面】高亮


    }
    public void Close_ItemMenu()
    {
        ItemPage.SetActive(false);
        CurrentChooseMenu = 1;
    }



    public GameObject MapPage;
    public void Open_MapMenu()
    {
        MapPage.SetActive(true);
        CurrentChooseMenu = 3;

    }
    public void Close_MapMenu()
    {
        MapPage.SetActive(false);
        CurrentChooseMenu = 1;
    }



    public GameObject SettingPage;
    public void Open_SettingMenu()
    {
        SettingPage.SetActive(true);
        CurrentChooseMenu = 5;
    }
    public void Close_SettingMenu()
    {
        SettingPage.SetActive(false);
        CurrentChooseMenu = 1;
    }




    [Header("目标文本(状态栏的时间)")]
    [SerializeField] private Text timeText; // 或 UnityEngine.UI.Text

    [Header("显示格式")]
    [SerializeField] private bool use24Hour = true;        // 24小时制
    [SerializeField] private string format24 = "HH:mm";    // 例如 12:00
    [SerializeField] private string format12 = "h:mm tt";  // 例如 12:00 AM/PM

    private WaitForSecondsRealtime wait;



    private IEnumerator UpdateClock()
    {
        while (true)
        {
            if (timeText)
            {
                DateTime now = DateTime.Now;
                string s = use24Hour
                    ? now.ToString(format24, CultureInfo.InvariantCulture)
                    : now.ToString(format12, CultureInfo.InvariantCulture);
                timeText.text = s;
            }
            yield return wait; // 实时计时，不受 timeScale 影响
        }
    }

    #endregion

    /// <summary>
    /// 物品栏
    /// </summary>
    #region

    public List<ItemOptionUI> itemButtons = new List<ItemOptionUI>();
    int ItemCurrentIndex = 0;

    public void ItemUnclockStart()
    {
        foreach (var btn in itemButtons)
            btn.RefreshFromSave(); // 检查解锁与数量

        // 查找第一个已解锁的物品
        for (int i = 0; i < itemButtons.Count; i++)
        {
            if (itemButtons[i].unlocked)
            {
                ItemCurrentIndex = i;
                break;
            }
        }

        UpdateHighlight_Item();
    }

    public void MoveSelection_Item(int step)
    {
        if (itemButtons.Count == 0) return;

        // 取消旧高亮
        itemButtons[ItemCurrentIndex].SetHighlight(false);

        int max = itemButtons.Count;
        int tries = 0;
        int idx = ItemCurrentIndex;

        do
        {
            idx = (idx + step + max) % max;
            tries++;
            if (itemButtons[idx].unlocked)
            {
                ItemCurrentIndex = idx;
                break;
            }
        } while (tries < max);

        UpdateHighlight_Item();
    }

    void UpdateHighlight_Item()
    {
        for (int i = 0; i < itemButtons.Count; i++)
            itemButtons[i].SetHighlight(i == ItemCurrentIndex);
    }


    [Header("物品详情UI")]
    public Image itemIcon;
    public Text itemNameText;
    public Text itemDescText;


    public void ShowItemInfo(ItemData data, int quantity)
    {
        if (data == null) { return; }
        if (quantity <= 0)
        {
            itemIcon.gameObject.SetActive(false);
            itemNameText.gameObject.SetActive(false);
            itemDescText.gameObject.SetActive(false);
        }
        else
        {
            itemIcon.gameObject.SetActive(true);
            itemNameText.gameObject.SetActive(true);
            itemDescText.gameObject.SetActive(true);
        }

        if (itemIcon) itemIcon.sprite = data.icon;
        if (itemNameText) itemNameText.text = data.displayName.Get(currentLocale);
        if (itemDescText) itemDescText.text = data.description.Get(currentLocale);
    }

    public bool IsCurrentItem(ItemOptionUI item)
    {
        return itemButtons.Count > 0
               && ItemCurrentIndex >= 0 && ItemCurrentIndex < itemButtons.Count
               && itemButtons[ItemCurrentIndex] == item;
    }

    // 你的 MoveSelection_Item 已有；确保它会跳过 unlocked=false 或 inactive 的项

    #endregion

    /// <summary>
    /// 菜单层面多端输入
    /// </summary>
    #region
    [SerializeField] private InputActionAsset inputActions;
    private InputAction moveAction;
    private InputAction confirmAction;
    private InputAction cancelAction;


    private InputAction pauseAction;

    public int PhonePage_currentIndex;// 0 Item  1 MAP   2 Photo    3 Setting   4 Gallery  5 MoveList  6 Phone  7 EMAIL
    public int CurrentChooseMenu;//-2对话AVG界面  -1商店界面  0 游戏界面  1 手机界面   2物品栏界面   3地图界面   5设置界面   

    private void OnEnable()
    {

        pauseAction = inputActions.FindAction("Pause");
        pauseAction.performed += OnPause;
        pauseAction.Enable();

        moveAction = inputActions.FindAction("Move");
        moveAction.performed += OnMove;
        moveAction.Enable();


        confirmAction = inputActions.FindAction("Attack");  // 或者用名为 "Submit"
        confirmAction.started += OnConfirm;
        confirmAction.Enable();


        cancelAction = inputActions.FindAction("Dodge");    // 或者用名为 "Cancel"
        cancelAction.started += OnCancel;
        cancelAction.Enable();



        //打开时钟
        wait = new WaitForSecondsRealtime(30f);   // 每秒更新一次；也可改成 30f/60f 省一点
        StartCoroutine(UpdateClock());
    }

    private void OnDisable()
    {
        pauseAction.started -= OnPause;
        pauseAction.Disable();

        moveAction.performed -= OnMove;
        moveAction.Disable();


        confirmAction.started -= OnConfirm;
        confirmAction.Disable();

        cancelAction.started -= OnCancel;
        cancelAction.Disable();




        //关闭时钟
        StopAllCoroutines();
    }

    //冷却时间
    private float inputCooldown2 = 0.2f;
    private float lastInputTime2 = -999f;

    private void OnMove(InputAction.CallbackContext ctx)
    {

        if (player.isInputBlocked == false) { return; }


        #region 冷却时间
        if (Time.time - lastInputTime2 < inputCooldown2)
            return;

        lastInputTime2 = Time.time;
        #endregion

        Vector2 dir = ctx.ReadValue<Vector2>();






        if (CurrentChooseMenu == -1)
        {


            // 当前菜单项内的上下切换
            if (dir.y > 0.5f)
            {

                MoveSelection_Shop(-1);

            }
            else if (dir.y < -0.5f)
            {
                MoveSelection_Shop(1);
            }
        }//商店界面



        if (CurrentChooseMenu == 1)
        {
            // 当前菜单项内的左右切换
            if (dir.x > 0.5f)
            {
                PhonePage_currentIndex = Mathf.Clamp(PhonePage_currentIndex + 1, 0, 7);
                UpdatePhonePage_Highlight();
            }
            else if (dir.x < -0.5f)
            {
                PhonePage_currentIndex = Mathf.Clamp(PhonePage_currentIndex - 1, 0, 7);
                UpdatePhonePage_Highlight();
            }

            // 当前菜单项内的上下切换
            if (dir.y > 0.5f)
            {
                PhonePage_currentIndex = Mathf.Clamp(PhonePage_currentIndex - 3, 0, 7);
                UpdatePhonePage_Highlight();

            }
            else if (dir.y < -0.5f)
            {
                PhonePage_currentIndex = Mathf.Clamp(PhonePage_currentIndex + 3, 0, 7);
                UpdatePhonePage_Highlight();
            }
        }//手机菜单主界面

        if (CurrentChooseMenu == 2)
        {
            // 当前菜单项内的左右切换
            if (dir.x > 0.5f)
            {
                MoveSelection_Item(+1);
            }
            else if (dir.x < -0.5f)
            {
                MoveSelection_Item(-1);
            }

            // 当前菜单项内的上下切换
            if (dir.y > 0.5f)
            {
                MoveSelection_Item(-3);
            }
            else if (dir.y < -0.5f)
            {
                MoveSelection_Item(+3);
            }

        }//物品栏界面


        if (CurrentChooseMenu == 5)
        {
            // 当前菜单项内的左右切换
            if (dir.x > 0.5f)
            {
                switch (SettingPagecurrentIndex)
                {
                    case 0:
                        BGM_Up();
                        break;
                    case 1:
                        SE_Up();
                        break;

                }

            }
            else if (dir.x < -0.5f)
            {

                switch (SettingPagecurrentIndex)
                {


                    case 0:
                        BGM_Down();
                        break;
                    case 1:
                        SE_Down();
                        break;

                }
            }


            // 当前菜单项内的上下切换
            if (dir.y > 0.5f)
            {
                SettingPagecurrentIndex = Mathf.Clamp(SettingPagecurrentIndex - 1, 0, 2);
                UpdateSettingPage_Highlight();


            }
            else if (dir.y < -0.5f)
            {
                SettingPagecurrentIndex = Mathf.Clamp(SettingPagecurrentIndex + 1, 0, 2);
                UpdateSettingPage_Highlight();


            }

        }//设置界面

        AudioManager.instance.AudioPlay(AudioManager.instance.Attack_pai1);


    }

    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        if (player.isInputBlocked == false) { return; }

        //对话AVG界面
        if (CurrentChooseMenu == -2)
        {
            dialogSystem.ShowText();//下一句
        }

        //商店界面
        if (CurrentChooseMenu == -1)
        {
            shopRows[shopIndex].Buy();
        }


        //手机主界面
        if (CurrentChooseMenu == 1)
        {
            switch (PhonePage_currentIndex)
            {
                case 0:
                    //进入物品栏菜单
                    Invoke(nameof(Open_ItemMenu), 0.2f);
                    break;
                case 1:
                    //进入地图菜单
                    Invoke(nameof(Open_MapMenu), 0.2f);
                    break;
                case 2:

                    break;
                case 3:
                    //进入设置界面
                    Invoke(nameof(Open_SettingMenu), 0.2f);
                    break;
            }

            AudioManager.instance.AudioPlay(AudioManager.instance.Attack_hit2);
        }

        //物品栏界面
        if (CurrentChooseMenu == 2)
        {
            itemButtons[ItemCurrentIndex].UseItem();

            AudioManager.instance.AudioPlay(AudioManager.instance.Attack_hit2);
        }






    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (player.isInputBlocked == false) { return; }

        //对话AVG界面
        if (CurrentChooseMenu == -2)
        {
            dialogSystem.ChangeStory();//跳过
        }

        if (CurrentChooseMenu == -1)
        {
            //退出商店
            Invoke(nameof(CloseShop), 0.2f);
        }


        if (CurrentChooseMenu == 1)
        {
            //退出手机
            Invoke(nameof(ClosePhone), 0.2f);

        }

        if (CurrentChooseMenu == 2)
        {
            //退出物品栏
            Invoke(nameof(Close_ItemMenu), 0.2f);
        }

        if (CurrentChooseMenu == 3)
        {
            //退出地图界面
            Invoke(nameof(Close_MapMenu), 0.2f);
        }

        if (CurrentChooseMenu == 5)
        {
            //退出地图界面
            Invoke(nameof(Close_SettingMenu), 0.2f);
        }

        AudioManager.instance.AudioPlay(AudioManager.instance.Attack_whip_1);

    }

    private void OnPause(InputAction.CallbackContext ctx)
    {


        if (Anim_Phone.GetBool("Open") == true)
        {
            ClosePhone();
        }
        else
        {
            OpenPhone();
        }

        AudioManager.instance.AudioPlay(AudioManager.instance.SE_Clothes);
    }




    [SerializeField] private Animator[] PhonePage_Animators; // 手机页面动画器数组
    private void UpdatePhonePage_Highlight()
    {
        for (int i = 0; i < PhonePage_Animators.Length; i++)
        {
            if (PhonePage_Animators[i] == null) continue;

            if (i == PhonePage_currentIndex)
                PhonePage_Animators[i].SetTrigger("Pressed");
            else
                PhonePage_Animators[i].SetTrigger("Normal");
        }
    }





    #endregion

    /// <summary>
    /// 商店系统/金币系统
    /// </summary>
    #region
    [Header("金币")]
    public Text MoneyText;
    public Text MoneyText_2;
    public void ChangeMoney(int amount, bool UseVoice = true)
    {
        // 取当前值
        SaveData _data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);
        int currentMoney = _data.Money;


        // 修改
        currentMoney += amount;
        if (currentMoney < 0) currentMoney = 0;   // 防止出现负数

        // ✅ 把修改结果写回存档对象
        _data.Money = currentMoney;
        // ✅ 保存回文件
        SaveManager.SaveGame(_data);

        // 更新 UI
        MoneyText.text = currentMoney.ToString();
        MoneyText_2.text = currentMoney.ToString();


        if (UseVoice) { AudioManager.instance.AudioPlay(AudioManager.instance.SE_Reji); }

    }


    [Header("商店类别")]
    public Image ShopImage, Shop_Line;
    public Sprite Shop_Sex, Shop_Common_1, Shop_Common_2, Shop_Line_1, Shop_Line_2;

    public GameObject ShopCavans;

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


        ShopCavans.SetActive(true);


        CurrentChooseMenu = -1;
        player.isInputBlocked = true;


        OpenShopWithPool(chosenPool);
    }

    public void CloseShop()
    {
        ShopCavans.SetActive(false);

        CurrentChooseMenu = 0;
        player.isInputBlocked = false;
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
        player.isInputBlocked = true;
        ShopCavans.SetActive(true);
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
    /// 启动AVG对话
    /// </summary>
    #region
    public DialogSystem dialogSystem;
    public void OpenAVG() 
    {
        dialogSystem.gameObject.SetActive(true);

        player.isInputBlocked = true;
        CurrentChooseMenu = -2;

        Invoke("StopAllEnemy", 1f);
    }

    void StopAllEnemy() 
    {
        // 寻找场景中所有其他敌人，设置围观
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in allEnemies)
        {
            e.GetComponent<Enemy>().state = EnemyState.Frozen; // 进入围观状态
            player.observingEnemies.Add(e.GetComponent<Enemy>());
        }
    }

    void AllowAllEnemy() 
    {
        // 清空所有围观敌人状态
        foreach (Enemy e in player.observingEnemies)
        {
            if (e != null)
            {
                e.state = EnemyState.Idle;
            }
        }
    }

    public void CloseAVG() 
    {
        Invoke("AllowAllEnemy", 1f);

        player.isInputBlocked = false;
        CurrentChooseMenu = 0;
    }
    #endregion







    #region   【寻找场景内残留脚本】
    [MenuItem("Tools/Cleanup/Remove All FaceToCamera Scripts")]
    static void RemoveAllFaceToCamera()
    {
        int count = 0;

        // 查找场景里所有 FaceToCamera 脚本
        FaceToCamera[] allFaces = GameObject.FindObjectsOfType<FaceToCamera>(true);

        foreach (var face in allFaces)
        {
            // 记录并销毁
            GameObject obj = face.gameObject;
           
            Debug.Log($"✅ Removed FaceToCamera from: {obj.name}");
            count++;
        }

        Debug.Log($"--- 共清理 {count} 个 FaceToCamera 组件 ---");
    }
#endregion
}
