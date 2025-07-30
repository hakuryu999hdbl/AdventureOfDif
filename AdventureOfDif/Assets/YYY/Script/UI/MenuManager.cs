using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static GrabbableObject;
using UnityEngine.SceneManagement;

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

    public void ChangeShowList(int ShowList)
    {
        //Settting版面内上下按钮在四个按钮中上下切换选中状态，

        // 更新当前选择索引
        CurrentChooseList = ShowList;

        // 控制三个 List 的显示和隐藏
        PlayerList.SetActive(ShowList == 0);
        SettingList.SetActive(ShowList == 1);
        ExitList.SetActive(ShowList == 2);

        // 切换按钮的选中状态
        switch (ShowList)
        {
            case 0:
                Play_Button.Select(); // 或 anim.SetTrigger("Selected")
                break;
            case 1:
                Setting_Button.Select();
                break;
            case 2:
                Exit_Button.Select();
                break;
        }
    } //默认显示0 PlayerList，显示一个List，另外两个List隐藏，左右方向键能够来回切换List//List状态显示为，对应Button动画器显示Selected

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

    private void OnEnable()
    {
        moveAction = inputActions.FindAction("Move");
        confirmAction = inputActions.FindAction("Attack");  // 或者用名为 "Submit"
        cancelAction = inputActions.FindAction("Dodge");    // 或者用名为 "Cancel"

        moveAction.performed += OnMove;
        confirmAction.started += OnConfirm;
        cancelAction.started += OnCancel;

        moveAction.Enable();
        confirmAction.Enable();
        cancelAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        confirmAction.started -= OnConfirm;
        cancelAction.started -= OnCancel;

        moveAction.Disable();
        confirmAction.Disable();
        cancelAction.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
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

            ChangeShowList(CurrentChooseList);//保持目前显示List
        }

        AudioManager.instance.AudioPlay(AudioManager.instance.Attack_pai1);
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
                            StartGame();
                            break;
                        case 1://存档2
                            StartGame();
                            break;
                        case 2://存档3
                            StartGame();
                            break;
                    }
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
    #endregion

    /// <summary>
    /// 存档统合
    /// </summary>
    #region
    [Header("存档界面UI")]
    public SaveSlotUI CurrentSaveSlotUI;

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
