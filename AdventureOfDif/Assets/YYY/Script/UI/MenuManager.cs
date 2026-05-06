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
        //AudioManager.Instance.PlayBGM(AudioManager.Instance.BGM_Theme, true);
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
        switch (Number) 
        {
            case 0:
                PlayList.SetActive(true);
                SettingList.SetActive(false);
                ExitList.SetActive(false);
                break;
            case 1:
                PlayList.SetActive(false);
                SettingList.SetActive(true);
                ExitList.SetActive(false);
                break;
            case 2:
                PlayList.SetActive(false);
                SettingList.SetActive(false);
                ExitList.SetActive(true);
                break;
        }
    }

    bool isSaveListOpen = false;
 
    public void OpenSave()
    {
        SaveList.SetActive(true);

        newGameButton.SetActive(false);


        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(saveFirstSelected);

        CurrentSaveSlotUI = saveFirstSelected.GetComponent<SaveSlotUI>();

        isSaveListOpen = true;
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (isSaveListOpen)
        {
            SaveList.SetActive(false);
            newGameButton.SetActive(true);
            

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(newGameButton);

            isSaveListOpen = false;
        }

    }//退出到开始菜单
    private void OnDeleteSave(InputAction.CallbackContext ctx)
    {
        if (!isSaveListOpen) return;

        if (CurrentSaveSlotUI == null) return;

        CurrentSaveSlotUI.OnDeleteClicked();
    }//删除当前存档

    [System.Obsolete]
    public void StartGame()
    {
        FindObjectOfType<SceneTransitionController>().StartGame("YYY");

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
