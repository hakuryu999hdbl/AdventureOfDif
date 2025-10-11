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


    public void OpenPhone()
    {
        Anim_Phone.SetBool("Open",true);

        Time.timeScale = 0;
    }

    public void ClosePhone()
    {
        Anim_Phone.SetBool("Open",false);
        Time.timeScale = 1;

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
    private InputAction deleteAction;

    private InputAction pauseAction;

    public int CurrentChooseList;
    private void OnEnable()
    {

        pauseAction = inputActions.FindAction("Pause");
        pauseAction.performed += OnPause;
        pauseAction.Enable();

        //  moveAction = inputActions.FindAction("Move");
        //  confirmAction = inputActions.FindAction("Attack");  // 或者用名为 "Submit"
        //  cancelAction = inputActions.FindAction("Dodge");    // 或者用名为 "Cancel"
        //  deleteAction = inputActions.FindAction("Run");    // 或者用名为 "Delete"
        //
        //  moveAction.performed += OnMove;
        //  confirmAction.started += OnConfirm;
        //  cancelAction.started += OnCancel;
        //  deleteAction.started += OnDelete;
        //
        //  moveAction.Enable();
        //  confirmAction.Enable();
        //  cancelAction.Enable();
        //  deleteAction.Enable();
    }

    private void OnDisable()
    {
        //moveAction.performed -= OnMove;
        //confirmAction.started -= OnConfirm;
        //cancelAction.started -= OnCancel;
        //deleteAction.started -= OnDelete;
        //pauseAction.started -= OnPause;
        //
        //
        //moveAction.Disable();
        //confirmAction.Disable();
        //cancelAction.Disable();
        //deleteAction.Disable();
        //
        //pauseAction.Disable();
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
        if (dir.x != 0)
        {
            if (dir.x > 0.5f)
            {
               
            }
            else if (dir.x < -0.5f)
            {
             
            }

        }
        else
        {
            


        }

        AudioManager.instance.AudioPlay(AudioManager.instance.Attack_pai1);


    }

    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        // 执行当前选中按钮的点击逻辑
        switch (CurrentChooseList)
        {
            case 0:

              
                break;
            case 1:



                break;
            case 2:
               
                break;
        }


        AudioManager.instance.AudioPlay(AudioManager.instance.Attack_hit2);
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {

        // 执行当前选中按钮的点击逻辑
        switch (CurrentChooseList)
        {
            case 0:

                break;
            case 1:

                break;
            case 2:

                break;
        }

        AudioManager.instance.AudioPlay(AudioManager.instance.SE_Glass);
    }

    private void OnDelete(InputAction.CallbackContext ctx)
    {

        // 执行当前选中按钮的点击逻辑
        switch (CurrentChooseList)
        {
            case 0:


                break;
            case 1:

                break;
            case 2:

                break;
        }

        //AudioManager.instance.AudioPlay(AudioManager.instance.SE_Glass);
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

        AudioManager.instance.AudioPlay(AudioManager.instance.SE_Glass);
    }


    #endregion


}
