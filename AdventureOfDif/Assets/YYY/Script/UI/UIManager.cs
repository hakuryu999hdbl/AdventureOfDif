using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
//using Unity.Android.Gradle.Manifest;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Pathfinding.RaycastModifier;
using static UnityEngine.Analytics.IAnalytic;


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
        OnStart();//多端输入

       

        ItemUnclockStart();//初始化【菜单界面】高亮

        //PlayerPrefs.SetInt("Cola",12);
        //PlayerPrefs.SetInt("ChocoBanana",4);
        //PlayerPrefs.SetInt("Pudding", 3);
        //PlayerPrefs.SetInt("Potion", 6);

        //初始无物品显示
        itemIcon.gameObject.SetActive(false);
        itemNameText.gameObject.SetActive(false);
        itemDescText.gameObject.SetActive(false);


      


        AudioManager.Instance.PlayBGM(AudioManager.Instance.BGM_Level_1 , true);


    }

    #endregion


    /// <summary>
    /// 手机菜单系统
    /// </summary>
    #region
    [Header("手机菜单系统")]
    public Animator Anim_Phone;
    public PlayerController playerController;

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




        //打开手机敌人不能动
        //StopAllEnemy();
    }
    public void ClosePhone()
    {
        Anim_Phone.SetBool("Open", false);

        //Time.timeScale = 1;

        CurrentChooseMenu = 0;


        //关掉手机敌人可以动
        //AllowAllEnemy();
    }





    public GameObject ItemPage;
    public void Open_ItemMenu()
    {
        ItemPage.SetActive(true);
        CurrentChooseMenu = 2;

        App.SetActive(false);
        firstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音
        EventSystem.current.SetSelectedGameObject(Back_Item);


      

        //TODO
        //UpdateHighlight_Item();
        //
        //ItemUnclockStart();//初始化【菜单界面】高亮

        //清空物品选中显示
        if (itemIcon) itemIcon.sprite = None;
        if (itemNameText) itemNameText.text = null;
        if (itemDescText) itemDescText.text = null;
    }
    public void Close_ItemMenu()
    {
        ItemPage.SetActive(false);
        CurrentChooseMenu = 1;

        EventSystem.current.SetSelectedGameObject(null);
        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音
        EventSystem.current.SetSelectedGameObject(firstSelected);

        App.SetActive(true);
    }



    public GameObject MapPage;
    public void Open_MapMenu()
    {
        MapPage.SetActive(true);
        CurrentChooseMenu = 3;

        firstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音
        EventSystem.current.SetSelectedGameObject(Back_Map);
    }
    public void Close_MapMenu()
    {
        MapPage.SetActive(false);
        CurrentChooseMenu = 1;

        EventSystem.current.SetSelectedGameObject(null);
        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音
        EventSystem.current.SetSelectedGameObject(firstSelected);

    }



    public GameObject SettingPage;
    public void Open_SettingMenu()
    {
        SettingPage.SetActive(true);
        CurrentChooseMenu = 5;

        firstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音
        EventSystem.current.SetSelectedGameObject(Back_Setting);
    }
    public void Close_SettingMenu()
    {
        SettingPage.SetActive(false);
        CurrentChooseMenu = 1;

        EventSystem.current.SetSelectedGameObject(null);
        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音
        EventSystem.current.SetSelectedGameObject(firstSelected);
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
    public Sprite None;//透明图片

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

    public int CurrentChooseMenu;//-2对话AVG界面  -1？？？  0 游戏界面  1 手机界面   2物品栏界面   3地图界面   5设置界面   






    private PlayerInputControl inputControl;
    public GameObject firstSelected;//打开手机第一个选中
    public GameObject Back_Map,Back_Setting,Back_Item;//打开对应界面的时候，当前选中变成了各自的退回按钮
    public GameObject App;//主菜单所有按钮合集，防止移动物品的当前选中过去

    void OnStart() 
    {
        inputControl = new PlayerInputControl();
        inputControl.UI.Cancel.started += OnCancel;


        //Invoke(nameof(DisableUIInput), 0.2f);
        //DisableUIInput();
    }

    private void OnEnable()
    {

        //pauseAction = inputActions.FindAction("Pause");
        //pauseAction.performed += OnPause;
        //pauseAction.Enable();
        //
        //moveAction = inputActions.FindAction("Move");
        //moveAction.performed += OnMove;
        //moveAction.Enable();
        //
        //
        //confirmAction = inputActions.FindAction("Attack");  // 或者用名为 "Submit"
        //confirmAction.started += OnConfirm;
        //confirmAction.Enable();
        //
        //
        //cancelAction = inputActions.FindAction("Dodge");    // 或者用名为 "Cancel"
        //cancelAction.started += OnCancel;
        //cancelAction.Enable();



        //打开时钟
        wait = new WaitForSecondsRealtime(30f);   // 每秒更新一次；也可改成 30f/60f 省一点
        StartCoroutine(UpdateClock());
    }

    private void OnDisable()
    {



       // pauseAction.started -= OnPause;
       // pauseAction.Disable();
       //
       // moveAction.performed -= OnMove;
       // moveAction.Disable();
       //
       //
       // confirmAction.started -= OnConfirm;
       // confirmAction.Disable();
       //
       // cancelAction.started -= OnCancel;
       // cancelAction.Disable();




        //关闭时钟
        StopAllCoroutines();
    }

    public void EnableUIInput()
    {
        inputControl.UI.Enable();
    }//UIManager暂停菜单打开调用
    public void DisableUIInput()
    {
        inputControl.UI.Disable();
    }//Player初始/暂停菜单关闭调用




    //冷却时间
    private float inputCooldown2 = 0.2f;
    private float lastInputTime2 = -999f;

    private void OnMove(InputAction.CallbackContext ctx)
    {

      

        #region 冷却时间
        if (Time.time - lastInputTime2 < inputCooldown2)
            return;

        lastInputTime2 = Time.time;
        #endregion

        Vector2 dir = ctx.ReadValue<Vector2>();











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


      

        //AudioManager.instance.AudioPlay(AudioManager.instance.Attack_pai1);


    }

    private void OnConfirm(InputAction.CallbackContext ctx)
    {
     
        //对话AVG界面
        if (CurrentChooseMenu == -2)
        {
            dialogSystem.ShowText();//下一句
        }

     


     

        //物品栏界面
        if (CurrentChooseMenu == 2)
        {
            itemButtons[ItemCurrentIndex].UseItem();

           //AudioManager.instance.AudioPlay(AudioManager.instance.Attack_hit2);
        }






    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
       


        //if (player.isInputBlocked == false) { return; }

        //对话AVG界面
        //if (CurrentChooseMenu == -2)
        //{
        //    dialogSystem.ChangeStory();//跳过
        //}
        //
        //if (CurrentChooseMenu == -1)
        //{
        //    //退出商店
        //    Invoke(nameof(CloseShop), 0.2f);
        //}
        //
        //
        if (CurrentChooseMenu == 1)
        {
            //退出手机
            //Invoke(nameof(ClosePhone), 0.2f);

            TogglePause();
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

        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);

    }

    private void OnPause(InputAction.CallbackContext ctx)
    {


        TogglePause();

        
    }

    public void TogglePause() 
    {
        if (Anim_Phone.GetBool("Open") == true)
        {
            ClosePhone();
            playerController.EnableGameplayInput();
            inputControl.Disable();
            
            firstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            OpenPhone();
            playerController.DisableGameplayInput();
            inputControl.Enable();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        //AudioManager.instance.AudioPlay(AudioManager.instance.SE_Clothes);
    }


    [SerializeField] private Animator[] PhonePage_Animators; // 手机页面动画器数组
  





    #endregion

   

    /// <summary>
    /// 启动AVG对话
    /// </summary>
    #region
    public DialogSystem dialogSystem;
    public void OpenAVG() 
    {
        dialogSystem.gameObject.SetActive(true);


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
            //player.observingEnemies.Add(e.GetComponent<Enemy>());
        }
    }

    void AllowAllEnemy() 
    {
        // 清空所有围观敌人状态
        //foreach (Enemy e in player.observingEnemies)
        //{
        //    if (e != null)
        //    {
        //        e.state = EnemyState.Idle;
        //    }
        //}
    }

    public void CloseAVG() 
    {
        Invoke("AllowAllEnemy", 1f);

      
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
