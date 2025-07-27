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

    public GameObject PlayerList, SettingList, ExitList;//各项列表
    public Button Play_Button, Setting_Button, Exit_Button;//按钮切换列表

    private void Start()
    {
        //默认选择PlayList
        Invoke("DelayPlayButon", 1f);

        BGM.instance.AudioPlayMenuMusic(-1);//播放主菜单背景音乐
    }


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
                //ChangeShowList_Setting(0);//默认选中GAME
                break;
            case 2:
                Exit_Button.Select();
                //ChangeShowList_Exit(0);//默认选中Yes
                break;
        }
    } //默认显示0 PlayerList，显示一个List，另外两个List隐藏，左右方向键能够来回切换List//List状态显示为，对应Button动画器显示Selected


    [Header("设置列表")]
    public List<Button> SettingButtons = new List<Button>();
    private int settingIndex = 0;
    void ChangeSettingIndex(int delta)
    {
        settingIndex = (settingIndex + delta + SettingButtons.Count) % SettingButtons.Count;
        SettingButtons[settingIndex].Select();
        //SettingButtons[exitIndex].GetComponent<Animator>().Play("Selected", 0, 0);
        SettingButtons[currentIndex].GetComponent<UIHighlightSwitcher>().ActivateThis();
    }


    [Header("退出列表")]
    public List<Button> ExitButtons = new List<Button>();
    private int exitIndex = 0;
    void ChangeExitIndex(int delta)
    {
        exitIndex = (exitIndex + delta + ExitButtons.Count) % ExitButtons.Count;
        ExitButtons[exitIndex].Select();
        //ExitButtons[exitIndex].GetComponent<Animator>().Play("Selected", 0, 0);
        ExitButtons[currentIndex].GetComponent<UIHighlightSwitcher>().ActivateThis();
    }





    public void StartGame()
    {

        SceneManager.LoadScene("YYY", LoadSceneMode.Single);

    }//开始游戏

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

    }

    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        // 执行当前选中按钮的点击逻辑
        switch (CurrentChooseList)
        {
            case 0: Play_Button.onClick.Invoke(); break;
            case 1: Setting_Button.onClick.Invoke(); break;
            case 2: Exit_Button.onClick.Invoke(); break;
        }
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        // 可选：退出菜单、返回上一级等
        Debug.Log("取消");
    }
    #endregion

}
