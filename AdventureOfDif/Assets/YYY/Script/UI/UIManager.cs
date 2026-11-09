using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
//using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Pathfinding.RaycastModifier;
using static UnityEngine.Analytics.IAnalytic;
using UnityEngine.SceneManagement;


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
    [Header("淫乱值")]
    public Image SexBar;

    [Header("暴击值")]
    public Image CriticalBar;
    public GameObject R;

    [Header("挣扎值")]
    public Image StruggleBar;
    public GameObject Struggle;
    public GameObject Struggle_Forbiden;

    [Header("倒地恢复")]
    public GameObject DownRecover;
    public Image DownRecoverBar;

    // 每按一次攻击键，减少多少秒倒地时间
    public float downRecoverPerPress = 0.5f;

    [Header("头像")]
    public Image HeadIcon;
    public Sprite HeadIcon_1, HeadIcon_2, HeadIcon_3;


    // 用于闪烁控制
    //private float flashTimer = 0f;
    //private bool flashOn = false;

    public void UpdateHeadIcon()
    {
        if (HeadIcon == null || playerController == null)
            return;

        if (playerController.isStruggling)
        {
            HeadIcon.sprite = HeadIcon_2;
            return;
        }

        if (playerController.character.currentHealth <
            playerController.character.maxHealth * 0.5f)
        {
            HeadIcon.sprite = HeadIcon_1;
            return;
        }

        HeadIcon.sprite = HeadIcon_3;
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
        //if (fillPercent > 0.9f)
        //{
        //    //flashTimer += Time.deltaTime * 4f; // 闪烁速度
        //    //float alpha = Mathf.Abs(Mathf.Sin(flashTimer));
        //    //Color flashColor = Color.Lerp(baseColor, Color.blue, alpha); 
        //    //CriticalBar.color = flashColor;
        //
        //
        //}
        //else
        //{
        //    //CriticalBar.color = baseColor;
        //
        //}


    }


    public void UpdateStruggleBar(int curAmount, int maxAmount)
    {
        StruggleBar.fillAmount = (float)curAmount / (float)maxAmount;


    }

    public void ShowStruggleBar_Forbiden()
    {
        Struggle_Forbiden.SetActive(true);
    }//单独显示禁止挣扎

    public void ShowStruggleBar()
    {
        Struggle.SetActive(true);
    }//单独显示挣扎

    public void HideStruggleBar()
    {
        Struggle.SetActive(false);
    }//单独隐藏挣扎



    public void ShowDownRecover()
    {
        if (DownRecover != null)
            DownRecover.SetActive(true);
    }//单独显示恢复

    public void HideDownRecover()
    {
        if (DownRecover != null)
            DownRecover.SetActive(false);
    }//单独隐藏恢复

    public void UpdateDownRecoverBar(float currentTime, float maxTime)
    {
        if (DownRecoverBar == null)
            return;

        if (maxTime <= 0f)
        {
            DownRecoverBar.fillAmount = 1f;
            return;
        }

        DownRecoverBar.fillAmount =
            1f - Mathf.Clamp01(currentTime / maxTime);
    }

    #endregion



    /// <summary>
    /// 区域背景音乐（Start在这里）
    /// </summary>
    #region

    public void Start()
    {


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
    /// 暂停菜单和可能未来用的到的物品栏还有时钟
    /// </summary>
    #region

    [Header("暂停菜单")]
    public GameObject PauseMenu;
    public GameObject Resume;

    public GameObject firstSelected;//打开暂停第一个选中

    public void OpenPauseMenu()
    {
        Debug.Log(
            "PauseMenu activeSelf: " + PauseMenu.activeSelf +
            " / activeInHierarchy: " + PauseMenu.activeInHierarchy +
            " / 父物体: " +
            (PauseMenu.transform.parent != null
                ? PauseMenu.transform.parent.name
                : "无")
        );

        firstSelected = EventSystem.current.currentSelectedGameObject;

        PauseMenu.SetActive(true);

        Time.timeScale = 0;

        StartCoroutine(SelectResumeNextFrame());

        CurrentChooseMenu = -1;
    }

    private IEnumerator SelectResumeNextFrame()
    {
        yield return null;

        if (!PauseMenu.activeInHierarchy)
            yield break;

        GameFlowData.suppressNextSelectSound = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(Resume);

        //Debug.Log("当前选中：" + EventSystem.current.currentSelectedGameObject?.name);
    }

    public void ClosePauseMenu()
    {
        PauseMenu.SetActive(false);

        GameFlowData.suppressNextSelectSound = true;

        // 不要继续选中已经隐藏的 Resume
        EventSystem.current.SetSelectedGameObject(null);

        // 恢复暂停前的选中项
        if (firstSelected != null && firstSelected.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        Time.timeScale = 1;

        CurrentChooseMenu = 0;
    }

    public void _BackToMenu()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }



    [Header("玩家")]
    public PlayerController playerController;

 


    public GameObject ItemPage;
    public void Open_ItemMenu()
    {
        ItemPage.SetActive(true);
        CurrentChooseMenu = 1;

   
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
        CurrentChooseMenu = 0;

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

    public int CurrentChooseMenu;//-2对话AVG界面  -1暂停菜单  0 游戏界面  1 物品栏界面     







    public GameObject Back_Item;//打开对应界面的时候，当前选中变成了各自的退回按钮


    private void OnEnable()
    {

        healthEventOn();

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


        healthEventOff();

        //关闭时钟
        StopAllCoroutines();
    }





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

    public void OnCancel()
    {
       


        //if (player.isInputBlocked == false) { return; }

        //对话AVG界面
        if (CurrentChooseMenu == -2)
        {
            dialogSystem.ChangeStory();//跳过
        }
        
        
        if (CurrentChooseMenu == 1)
        {
            //退出物品栏
            Invoke(nameof(Close_ItemMenu), 0.2f);


        }
        
      

        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);

    }

    public void TogglePause() 
    {
        if (Time.unscaledTime - lastPauseTime < pauseCooldown)
            return;
        lastPauseTime = Time.unscaledTime;

        Debug.Log("【Dif】TogglePause执行，当前状态：" + PauseMenu.activeSelf);
        if (PauseMenu.activeSelf)
        {
            ClosePauseMenu();
            playerController.EnableGameplayInput();
        }
        else
        {
            OpenPauseMenu();
            playerController.DisableGameplayInput();
        }

        //AudioManager.instance.AudioPlay(AudioManager.instance.SE_Clothes);
    }

    private float lastPauseTime = -999f;
    private const float pauseCooldown = 0.2f;






    #endregion



    /// <summary>
    /// 启动AVG对话
    /// </summary>
    #region
    [Header("战后AVG")]
    public OpenAVG clearAVG;

    public void SaveOpenAVG(OpenAVG AVG) 
    {
        clearAVG = AVG;
    }


    public DialogSystem dialogSystem;
    private NpcController currentNpc;

    public void OpenAVG(int Number, NpcController npc = null) 
    {
        currentNpc = npc;

        CurrentChooseMenu = -2;

        playerController.DisableGameplayInput();

        dialogSystem.animation_number = Number;
        dialogSystem.gameObject.SetActive(true);

        Invoke(nameof(SuspendAllEnemiesForAVG), 0.5f);
    }




    private readonly List<EnemyController> avgPausedEnemies =
    new List<EnemyController>();


    private void SuspendAllEnemiesForAVG()
    {
        avgPausedEnemies.Clear();

        EnemyController[] enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);

        foreach (EnemyController enemy in enemies)
        {
            if (enemy == null || enemy.isAVGPaused)
                continue;

            enemy.SuspendForAVG();
            avgPausedEnemies.Add(enemy);

            Debug.Log(
            "暂停敌人：" + enemy.name +
            " / paused=" + enemy.isAVGPaused +
            " / AI=" + enemy.aiPath.canMove +
            " / AnimSpeed=" + enemy.anim.speed
        );
        }
    }// 寻找场景中所有其他敌人，设置围观

    private void ResumeAllEnemiesFromAVG()
    {
        foreach (EnemyController enemy in avgPausedEnemies)
        {
            if (enemy != null)
                enemy.ResumeFromAVG();
        }

        avgPausedEnemies.Clear();
    } // 清空所有围观敌人状态




    public void CloseAVG() 
    {

        // 让当前对话的 NPC 返回待机
        if (currentNpc != null)
        {
            currentNpc.StopTalk();
            currentNpc = null;
        }

        ResumeAllEnemiesFromAVG();

        playerController.EnableGameplayInput();
        CurrentChooseMenu = 0;
    }


    #endregion




    /// <summary>
    /// 生命值，体力值等UI
    /// </summary>
    #region

    [Header("事件监听")]
    public CharacterEventSO healthEvent;

    private void healthEventOn()
    {
        healthEvent.OnEventRaised += OnHealthEvent;

    }
    private void healthEventOff()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;

    }

    void OnHealthEvent(Character character)
    {
        var persentage = character.currentHealth / character.maxHealth;//将百分比传输
        OnHealthChange(persentage);

    }






    public Image healthImage;
    public Image healthDelayImage;
 

    public void OnHealthChange(float persentage)
    {
        healthImage.fillAmount = persentage;
    }

   
    private void Update()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime * 1.2f;//可以调整速度
        }


    }
    #endregion


}
