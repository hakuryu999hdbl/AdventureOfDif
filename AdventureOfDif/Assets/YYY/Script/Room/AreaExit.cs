using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaExit : MonoBehaviour
{
    private bool triggered = false;

    RoomGenerator RoomGenerator;//寻找RoomGenerator

    public String ExitName;

    public int ShopNumber = 0;//0离开场景出口   1超市一  2超市二   3涩情超市

    void Start() 
    {
        //寻找RoomGenerator
        RoomGenerator = GameObject.FindGameObjectWithTag("RoomGenerator").GetComponent<RoomGenerator>();
    }

    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            triggered = true;


            switch (ShopNumber) 
            {
                case 0:
                    GameFlowData.nextAreaId = ExitName;//记录下一个前往区域
                    RoomGenerator.LoadNextArea();
                    break;

                case 1:
                    UIManager.instance.OpenShop(1);
                    triggered = false;
                    break;

                case 2:
                    UIManager.instance.OpenShop(2);
                    triggered = false;
                    break;

                case 3:
                    UIManager.instance.OpenShop(3);
                    triggered = false;
                    break;
            }


         
        }
    }
}