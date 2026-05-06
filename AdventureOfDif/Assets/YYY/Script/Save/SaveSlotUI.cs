using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour, ISelectHandler
{
    public MenuManager menuManager;//刷场景需要

    public string slotName; // "CurrentPlayer1", "CurrentPlayer2", "CurrentPlayer3"

    public Text nameText, timeText;
    //public Text nextAreaIdText;//位于区域？
    public Image thumbnail;


    public Sprite defaultThumbnail, Thumbnail_1;


    public GameObject X_Button;

    public void OnSelect(BaseEventData eventData)
    {
        menuManager.CurrentSaveSlotUI = this;
    }//监听自己是当前选中按钮


    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (SaveManager.Exists(slotName))
        {
            //这个槽位有存档

            SaveData data = SaveManager.LoadGame(slotName);
            nameText.text = slotName;
            timeText.text = data.saveTime;
            //nextAreaIdText.text = data.NextAreaId;

            thumbnail.sprite = Thumbnail_1; // 以后可以换成 data.thumbnail

            X_Button.SetActive(true);
        }
        else
        {
            //这个槽位无存档

            nameText.text = "Unnamed";
            timeText.text = "--------------------";
            //nextAreaIdText.text = "";


            thumbnail.sprite = defaultThumbnail; // 以后可以换成 data.thumbnail

            X_Button.SetActive(false);

        }
    }

    public void OnLoadClicked()
    {
        if (SaveManager.Exists(slotName))
        {
            //点击读取存档

            // 先加载存档数据
            SaveData data = SaveManager.LoadGame(slotName);

         
            GameFlowData.CurrentPlayer = slotName;//临时储存当前是哪个档

            // 跳转游戏主场景
            menuManager.StartGame();

        }
        else
        {
            //新建存档
            //UIManager.instance.SaveNameMenu.SetActive(true);
            menuManager.CurrentSaveSlotUI = this;
            menuManager.OnConfirmNameInput();

        }


    }//点击按钮

    public void OnDeleteClicked()
    {
        if (SaveManager.Exists(slotName))
        {
            SaveManager.DeleteGame(slotName);
            Refresh(); // UI刷新
        }
    }//被删除
}
