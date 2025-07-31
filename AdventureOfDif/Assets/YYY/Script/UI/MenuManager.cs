using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static GrabbableObject;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    /// <summary>
    /// 主菜单
    /// </summary>
    #region
    [Header("主菜单列表")]
    public int CurrentChooseList = 0;//0 PlayerList  1Setting  2ExitList
    public int currentIndex = 0;//0 Play  1 GAME    2 CONTROL     3 VOICE     4 LANGUACE   5 Exit_Yes  6 Exit_No

    public GameObject PlayerList, SettingList, ExitList, SaveList;//各项列表
    public Button Play_Button, Setting_Button, Exit_Button;//按钮切换列表

    private void Start()
    {
        //默认选择PlayList
        Invoke("DelayPlayButon", 1f);

        BGM.instance.AudioPlayMenuMusic(-1);//播放主菜单背景音乐
    }

    public Button PlayButton;

    public void DelayPlayButon()
    {
        ChangeShowList(0);

    }//开始默认选中Play


    //冷却时间
    private float inputCooldown = 0.2f;
    private float lastInputTime = -999f;


    public void ChangeShowList(int ShowList)
    {

        #region 冷却时间
        if (Time.time - lastInputTime < inputCooldown)
            return;

        lastInputTime = Time.time;
        #endregion

        //Settting版面内上下按钮在四个按钮中上下切换选中状态，

        // 更新当前选择索引
        CurrentChooseList = ShowList;

        // 控制三个 List 的显示和隐藏
        PlayerList.SetActive(ShowList == 0);
        SettingList.SetActive(ShowList == 1);
        ExitList.SetActive(ShowList == 2);

        // 延迟一帧选中，避免在 SetActive 后 UI 还未准备好
        StartCoroutine(DelayedSelect(ShowList));

        // 切换按钮的选中状态
        //switch (ShowList)
        //{
        //    case 0:
        //        EventSystem.current.SetSelectedGameObject(Play_Button.gameObject); // 或 anim.SetTrigger("Selected")
        //        break;
        //    case 1:
        //        EventSystem.current.SetSelectedGameObject(Setting_Button.gameObject);
        //        break;
        //    case 2:
        //        EventSystem.current.SetSelectedGameObject(Exit_Button.gameObject);
        //        break;
        //}
    } //默认显示0 PlayerList，显示一个List，另外两个List隐藏，左右方向键能够来回切换List//List状态显示为，对应Button动画器显示Selected

    private IEnumerator DelayedSelect(int ShowList)
    {
        yield return null;

        switch (ShowList)
        {
            case 0:
                EventSystem.current.SetSelectedGameObject(Play_Button.gameObject);
                break;
            case 1:
                EventSystem.current.SetSelectedGameObject(Setting_Button.gameObject);
                break;
            case 2:
                EventSystem.current.SetSelectedGameObject(Exit_Button.gameObject);
                break;
        }
    }


    public int PlayerList_Index = 0;//0开始游戏的图标  1存档界面图标

    [Header("存档列表")]
    public List<Button> SaveButtons = new List<Button>();
    public List<GameObject> SaveButtons_Hightlight = new List<GameObject>();
    private int saveIndex = 0;
    void ChangeSaveIndex(int delta)
    {
        saveIndex = (saveIndex + delta + SaveButtons.Count) % SaveButtons.Count;
        SaveButtons[saveIndex].Select();

        foreach (var obj in SaveButtons_Hightlight)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        SaveButtons_Hightlight[saveIndex].SetActive(true);
    }


    [Header("设置列表")]
    public List<Button> SettingButtons = new List<Button>();
    public List<GameObject> SettingButtons_Hightlight = new List<GameObject>();
    private int settingIndex = 0;
    void ChangeSettingIndex(int delta)
    {
        settingIndex = (settingIndex + delta + SettingButtons.Count) % SettingButtons.Count;
        SettingButtons[settingIndex].Select();

        foreach (var obj in SettingButtons_Hightlight)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        SettingButtons_Hightlight[settingIndex].SetActive(true);
    }


    [Header("退出列表")]
    public List<Button> ExitButtons = new List<Button>();
    public List<GameObject> ExitButtons_Hightlight = new List<GameObject>();
    private int exitIndex = 0;
    void ChangeExitIndex(int delta)
    {
        exitIndex = (exitIndex + delta + ExitButtons.Count) % ExitButtons.Count;
        ExitButtons[exitIndex].Select();


        foreach (var obj in ExitButtons_Hightlight)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        ExitButtons_Hightlight[exitIndex].SetActive(true);
    }


    [System.Obsolete]
    public void StartGame()
    {
        FindObjectOfType<SceneTransitionController>().StartGame("YYY");

        //SceneManager.LoadScene("YYY", LoadSceneMode.Single);

    }//开始游戏

    public void OpenSave() 
    {
        PlayButton.Select();

        PlayerList_Index = 1;
        PlayButton.gameObject.SetActive(false);
        SaveList.SetActive(true);
    }//打开存档界面

    public void Exit_Yes() 
    {
        Application.Quit();
    }//退出游戏
    public void Exit_No() 
    {
        ChangeShowList(0);
    }//返回进入游戏状态

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

    private void OnEnable()
    {
        moveAction = inputActions.FindAction("Move");
        confirmAction = inputActions.FindAction("Attack");  // 或者用名为 "Submit"
        cancelAction = inputActions.FindAction("Dodge");    // 或者用名为 "Cancel"
        deleteAction = inputActions.FindAction("Run");    // 或者用名为 "Delete"

        moveAction.performed += OnMove;
        confirmAction.started += OnConfirm;
        cancelAction.started += OnCancel;
        deleteAction.started += OnDelete;

        moveAction.Enable();
        confirmAction.Enable();
        cancelAction.Enable();
        deleteAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        confirmAction.started -= OnConfirm;
        cancelAction.started -= OnCancel;
        deleteAction.started -= OnDelete;

        moveAction.Disable();
        confirmAction.Disable();
        cancelAction.Disable();
        deleteAction.Disable();
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
                ChangeShowList((CurrentChooseList + 1) % 3);
            }
            else if (dir.x < -0.5f)
            {
                ChangeShowList((CurrentChooseList + 2) % 3); // 相当于 -1
            }
            // 若之后你有上下功能，可以加 dir.y 判断
        }
        else
        {
            // 当前菜单项内的上下切换
            switch (CurrentChooseList)
            {
                case 0://PlayerList
                    if (PlayerList_Index == 0)
                    {
                        PlayButton.Select();
                    }
                    else
                    {
                        if (dir.y > 0.5f)
                            ChangeSaveIndex(-1);
                        else if (dir.y < -0.5f)
                            ChangeSaveIndex(1);
                    }

                    break;
                case 1: // SettingList
                    if (dir.y > 0.5f)
                        ChangeSettingIndex(-1);
                    else if (dir.y < -0.5f)
                        ChangeSettingIndex(1);
                    break;

                case 2: // ExitList
                    if (dir.y > 0.5f)
                        ChangeExitIndex(-1);
                    else if (dir.y < -0.5f)
                        ChangeExitIndex(1);
                    break;
            }

           
        }

        AudioManager.instance.AudioPlay(AudioManager.instance.Attack_pai1);

        ChangeShowList(CurrentChooseList);//保持目前显示List
    }

    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        // 执行当前选中按钮的点击逻辑
        switch (CurrentChooseList)
        {
            case 0:

                if (PlayerList_Index==0) 
                {
                    OpenSave();
                }
                else
                {
                    switch (saveIndex)
                    {
                        case 0://存档1
                            CurrentSaveSlotUI = Save_1;                         
                            break;
                        case 1://存档2
                            CurrentSaveSlotUI = Save_2;
                            break;
                        case 2://存档3
                            CurrentSaveSlotUI = Save_3;
                            break;
                    }

                    CurrentSaveSlotUI.OnLoadClicked();
                }   
                break;
            case 1:

                switch (settingIndex) 
                {
                    case 0://game
                        break;
                    case 1://control
                        break;
                    case 2://voice
                        break;
                    case 3://language
                        break;
                }



                break;
            case 2:
                switch (exitIndex)
                {
                    case 0://yes
                        Exit_Yes();
                        break;
                    case 1://no
                        Exit_No();
                        break;
                }
                break;
        }

        ChangeShowList(CurrentChooseList);//保持目前显示List
        AudioManager.instance.AudioPlay(AudioManager.instance.Attack_hit2);
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        // 可选：退出菜单、返回上一级等
        //Debug.Log("取消");

        // 执行当前选中按钮的点击逻辑
        switch (CurrentChooseList)
        {
            case 0:

                PlayerList_Index = 0;
                PlayButton.gameObject.SetActive(true);
                SaveList.SetActive(false);
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
        // 可选：退出菜单、返回上一级等
        //Debug.Log("取消");

        // 执行当前选中按钮的点击逻辑
        switch (CurrentChooseList)
        {
            case 0:

                if (PlayerList_Index == 1)
                {
                    switch (saveIndex)
                    {
                        case 0://删除存档1
                            CurrentSaveSlotUI = Save_1;
                            break;
                        case 1://删除存档2
                            CurrentSaveSlotUI = Save_2;
                            break;
                        case 2://删除存档3
                            CurrentSaveSlotUI = Save_3;
                            break;
                    }

                    CurrentSaveSlotUI.OnDeleteClicked();
                }

                break;
            case 1:

                break;
            case 2:

                break;
        }

        //AudioManager.instance.AudioPlay(AudioManager.instance.SE_Glass);
    }
    #endregion

    /// <summary>
    /// 存档统合
    /// </summary>
    #region
    [Header("存档界面UI")]
    public SaveSlotUI CurrentSaveSlotUI;
    public SaveSlotUI Save_1, Save_2, Save_3;


    public void OnConfirmNameInput()
    {
        if (CurrentSaveSlotUI != null)
        {

            // 新建存档
            SaveData newData = new SaveData(CurrentSaveSlotUI.slotName);

            newData.slotName = CurrentSaveSlotUI.slotName;//记住档的名字


            SaveManager.SaveGame(newData);

            CurrentSaveSlotUI.Refresh();//更新当前存档内容
        }

    }//玩家确定这个存档名称


    public void OpenSaveURL()
    {
        Application.OpenURL(Application.persistentDataPath);
    }//打开存档位置文件夹
    #endregion
}
