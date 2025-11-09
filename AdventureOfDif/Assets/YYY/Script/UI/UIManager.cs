using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }
    void Awake()
    {
        instance = this;

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
    /// 区域背景音乐
    /// </summary>
    #region

    public void Start()
    {
        PlayRegionBGM(1);

        UpdatePhonePage_Highlight(); // 初始化【手机主界面】高亮

        ItemUnclockStart();//初始化【菜单界面】高亮
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
    /// 手机菜单系统
    /// </summary>
    #region

    public Animator Anim_Phone;

    public Player player;

    public void OpenPhone()
    {
        Anim_Phone.SetBool("Open", true);

        //Time.timeScale = 0;

        CurrentChooseMenu = 1;
        player.isInputBlocked = true;
    }
    public void ClosePhone()
    {
        Anim_Phone.SetBool("Open", false);

        //Time.timeScale = 1;

        CurrentChooseMenu = 0;
        player.isInputBlocked = false;
    }





    public GameObject ItemPage;
    public void Open_ItemMenu() 
    {
        ItemPage.SetActive(true);
        CurrentChooseMenu = 2;
    }
    public void Close_ItemMenu()
    {
        ItemPage.SetActive(false);
        CurrentChooseMenu = 1;
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

    void MoveSelection_Item(int step)
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
    public int CurrentChooseMenu;// 0 游戏界面  1 手机界面   2物品栏界面

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

        if (CurrentChooseMenu==1) 
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
        }

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

        }

        AudioManager.instance.AudioPlay(AudioManager.instance.Attack_pai1);


    }

    private void OnConfirm(InputAction.CallbackContext ctx)
    {
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

                    break;
                case 2:

                    break;
            }
        }

        if (CurrentChooseMenu == 2)
        {
           // itemButtons[ItemCurrentIndex].UseItem();
        }
        




        AudioManager.instance.AudioPlay(AudioManager.instance.Attack_hit2);
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {

        //手机主界面
        if (CurrentChooseMenu == 1) 
        {
            switch (PhonePage_currentIndex)
            {
                case 0:
                    //退出手机
                    Invoke(nameof(ClosePhone), 0.2f);
                    break;
                case 1:
                  
                    break;
                case 2:

                    break;
            }
           
        }

        if (CurrentChooseMenu == 2)
        {
            //退出物品栏
            Invoke(nameof(Close_ItemMenu), 0.2f);
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


   
}
