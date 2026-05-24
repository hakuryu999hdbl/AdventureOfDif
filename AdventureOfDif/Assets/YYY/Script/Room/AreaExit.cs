using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaExit : MonoBehaviour
{


    //RoomGenerator RoomGenerator;//寻找RoomGenerator

    public String ExitName;

    public int ShopNumber = 0;//0离开场景出口   1超市一  2超市二   3涩情超市

    void Start() 
    {
        //寻找RoomGenerator
        //RoomGenerator = GameObject.FindGameObjectWithTag("RoomGenerator").GetComponent<RoomGenerator>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
 


            switch (ShopNumber) 
            {
                case 0:
                    GameFlowData.nextAreaId = ExitName;//记录下一个前往区域
                    RoomGenerator.instance.LoadNextArea();
                    break;

                case 1:
                    GameFlowData.ShopType = 1;
                    GameFlowData.nextAreaId = ExitName;//记录下一个前往区域
                    RoomGenerator.instance.LoadShop();
                    break;

                case 2:
                    GameFlowData.ShopType = 2;
                    GameFlowData.nextAreaId = ExitName;//记录下一个前往区域
                    RoomGenerator.instance.LoadShop();
                    break;

                case 3:
                    GameFlowData.ShopType = 3;
                    GameFlowData.nextAreaId = ExitName;//记录下一个前往区域
                    RoomGenerator.instance.LoadShop();
                    break;
            }


         
        }
    }
}