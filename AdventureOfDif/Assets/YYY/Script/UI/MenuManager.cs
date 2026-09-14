using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{


    /// <summary>
    /// 菜单层面多端输入
    /// </summary>
    #region
    [Header("多端输入")]
    public GameObject newGameButton;//开头默认选中
    public GameObject saveFirstSelected;//打开存档界面首个选中
    private PlayerInputControl inputControl;

    private void Awake()
    {
        inputControl = new PlayerInputControl();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(newGameButton);//开头设置默认按钮

        inputControl.UI.Cancel.started += OnCancel;
        inputControl.UI.Delete.started += OnDeleteSave;

        //InitLanguageOnce();//根据系统设置语言


        //Debug.Log("目前是否根据系统语言进行设置" + PlayerPrefs.GetInt("language_initialized"));//0无设置  1已经设置好
        Debug.Log("目前储存的语言" + PlayerPrefs.GetInt("language"));//0日语 1简体中文 2繁体中文 3英语 4韩语


    }
    private void Start()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.BGM_Theme, true);
    }
    private void OnEnable()
    {

        inputControl.Enable();

    }

    private void OnDisable()
    {
        inputControl.Disable();
    }
   


    public GameObject PlayList, SettingList, ExitList,SaveList;
    public void ChangeShowList(int Number) 
    {

        //不管现在在哪个二级菜单，
        //点击底部 PLAY / SETTING / EXIT 都直接强制关闭
        ForceCloseAllSettingSubMenu();

        switch (Number) 
        {
            case 0:
                PlayList.SetActive(true);
                SettingList.SetActive(false);
                ExitList.SetActive(false);

                CurrentOpen = 0;
                break;
            case 1:
                PlayList.SetActive(false);
                SettingList.SetActive(true);
                ExitList.SetActive(false);

                CurrentOpen = 1;
                break;
            case 2:
                PlayList.SetActive(false);
                SettingList.SetActive(false);
                ExitList.SetActive(true);

                CurrentOpen = -2;
                break;
        }
    }

 
    public void OpenSave()
    {
        SaveList.SetActive(true);

        newGameButton.SetActive(false);


        EventSystem.current.SetSelectedGameObject(null);
        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音
        EventSystem.current.SetSelectedGameObject(saveFirstSelected);

        CurrentSaveSlotUI = saveFirstSelected.GetComponent<SaveSlotUI>();

        CurrentOpen = -1;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {

        switch (CurrentOpen)
        {
            case -1:

                SaveList.SetActive(false);
                newGameButton.SetActive(true);


                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(newGameButton);

                CurrentOpen = 0;

                break;


            //设置二级菜单
            case 2:
            case 3:
            case 4:
            case 5:

                CloseSettingSubMenu();
                break;


                //以后这里可以继续追加
                //case 1:
                //    CloseSetting();
                //    break;
        }


    }//退出到开始菜单
    private void OnDeleteSave(InputAction.CallbackContext ctx)
    {
        if (CurrentOpen !=-1) return;

        if (CurrentSaveSlotUI == null) return;

        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Click);


        CurrentSaveSlotUI.OnDeleteClicked();
    }//删除当前存档


    public void StartGame()
    {

        GameFlowData.ClearRunData();//进入新游戏前清空状态（假如有离开菜单回主菜单，再度重新进入游戏路径）

        FindFirstObjectByType<SceneTransitionController>().StartGame("YYY");

        //SceneManager.LoadScene("YYY", LoadSceneMode.Single);



    }//开始游戏


    public void Exit_Yes()
    {
        Application.Quit();
    }//退出游戏
    public void Exit_No()
    {

    }//返回进入游戏状态

    #endregion


    /// <summary>
    /// 设置二级菜单
    /// </summary>
    #region

    [Header("设置二级菜单")]
    public GameObject ResetMenu;
    public GameObject ScreenMenu;
    public GameObject VoiceMenu;
    public GameObject LanguageMenu;

    [Header("二级菜单 Back")]
    public GameObject ResetBack;
    public GameObject ScreenBack;
    public GameObject VoiceBack;
    public GameObject LanguageBack;

    [Header("设置主页按钮")]
    public GameObject ResetButton;
    public GameObject ScreenButton;
    public GameObject VoiceButton;
    public GameObject LanguageButton;


    //当前打开的菜单

    //-2 是否退出
    //-1 存档界面
    //0 主菜单
    //1 设置主页
    //2 Reset
    //3 Screen
    //4 Voice
    //5 Language
    private int CurrentOpen = 0;


    //打开 RESET
    public void OpenReset()
    {
        OpenSettingSubMenu(
            ResetMenu,
            ResetBack,
            2
        );
    }


    //打开 SCREEN
    public void OpenScreen()
    {
        OpenSettingSubMenu(
            ScreenMenu,
            ScreenBack,
            3
        );
    }


    //打开 VOICE
    public void OpenVoice()
    {
        OpenSettingSubMenu(
            VoiceMenu,
            VoiceBack,
            4
        );
    }


    //打开 LANGUAGE
    public void OpenLanguage()
    {
        OpenSettingSubMenu(
            LanguageMenu,
            LanguageBack,
            5
        );
    }


    //统一打开二级菜单
    private void OpenSettingSubMenu(
        GameObject menu,
        GameObject backButton,
        int menuNumber
    )
    {
        menu.SetActive(true);

        //设置主页隐藏
        SettingList.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(backButton);

        CurrentOpen = menuNumber;
    }


    //关闭二级菜单
    public void CloseSettingSubMenu()
    {
        GameObject returnButton = null;

        switch (CurrentOpen)
        {
            case 2:

                ResetMenu.SetActive(false);
                returnButton = ResetButton;

                break;

            case 3:

                ScreenMenu.SetActive(false);
                returnButton = ScreenButton;

                break;

            case 4:

                VoiceMenu.SetActive(false);
                returnButton = VoiceButton;

                break;

            case 5:

                LanguageMenu.SetActive(false);
                returnButton = LanguageButton;

                break;
        }

        SettingList.SetActive(true);

        EventSystem.current.SetSelectedGameObject(null);

        if (returnButton != null)
        {
            EventSystem.current.SetSelectedGameObject(returnButton);
        }

        CurrentOpen = 1;
    }

    private void ForceCloseAllSettingSubMenu()
    {
        ResetMenu.SetActive(false);
        ScreenMenu.SetActive(false);
        VoiceMenu.SetActive(false);
        LanguageMenu.SetActive(false);
    }// 强制关闭所有设置二级菜单//不处理选中对象，不修改CurrentOpen


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


    private void Update()
    {
        if (Keyboard.current.f8Key.wasPressedThisFrame)
        {
            OpenSaveURL();
        }
    }



    public void Delete_All() 
    {
        Save_1.OnDeleteClicked();
        Save_2.OnDeleteClicked();
        Save_3.OnDeleteClicked();

        PlayerPrefs.DeleteAll();

    }
    #endregion


    /// <summary>
    /// 转到外部网站
    /// </summary>
    #region
    public void OpenTwitter()
    {
        Application.OpenURL("https://x.com/Detective_ye");
    }
    public void OpenCi_en()
    {
        Application.OpenURL("https://ci-en.dlsite.com/creator/16247");
    }
    public void OpenPixiv()
    {
        Application.OpenURL("https://www.pixiv.net/users/38416908");
    }
    public void OpenDLsite()
    {
        Application.OpenURL("https://www.dlsite.com/maniax/work/=/product_id/RJ01296940.html");
    }
    public void OpenFanza()
    {
        Application.OpenURL("https://www.dmm.co.jp/dc/doujin/-/detail/=/cid=d_480255/?utm_source=twitter&utm_medium=social_tpost&utm_campaign=start&utm_term=d_480255&utm_content=doujin");
    }
    public void OpenSteam()
    {
        Application.OpenURL("https://store.steampowered.com/app/3297870/_/?beta=0");
    }
    #endregion
}
