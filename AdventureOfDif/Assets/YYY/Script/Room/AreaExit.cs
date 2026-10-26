using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaExit : MonoBehaviour, IInteractable
{


    //RoomGenerator RoomGenerator;//寻找RoomGenerator

    public String ExitName;

    public int ShopNumber = 0;//0离开场景出口   1超市一  2超市二   3涩情超市

    private bool isLoading;

    void Start() 
    {
        //寻找RoomGenerator
        //RoomGenerator = GameObject.FindGameObjectWithTag("RoomGenerator").GetComponent<RoomGenerator>();
    }


    public void TriggerAction()
    {
        if (isLoading || RoomGenerator.instance == null) return;
        if (string.IsNullOrEmpty(ExitName))
        {
            Debug.LogWarning($"{name}: AreaExit 的 ExitName 未设置", this);
            return;
        }

        if (ShopNumber < 0 || ShopNumber > 3)
        {
            Debug.LogWarning($"{name}: 未知的 ShopNumber = {ShopNumber}", this);
            return;
        }

        isLoading = true;
        GameFlowData.nextAreaId = ExitName;

        if (ShopNumber == 0)
        {
            RoomGenerator.instance.LoadNextArea();
        }
        else
        {
            GameFlowData.ShopType = ShopNumber;
            RoomGenerator.instance.LoadShop();
        }
    }
}