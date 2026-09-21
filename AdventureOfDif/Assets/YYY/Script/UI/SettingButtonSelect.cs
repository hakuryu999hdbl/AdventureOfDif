using UnityEngine;
using UnityEngine.EventSystems;

public class SettingButtonSelect : MonoBehaviour, ISelectHandler
{
    public GameObject targetMenu;

    public bool MainButton = false;

    public void OnSelect(BaseEventData eventData)
    {
        MenuManager menu = FindFirstObjectByType<MenuManager>();

        if (!MainButton)
        {
            menu.PreviewSettingMenu(targetMenu);
        }
        else
        {
            menu.HideSettingMenu();
        }
      
    }
}