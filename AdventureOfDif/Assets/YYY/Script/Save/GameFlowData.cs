using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameFlowData 
{
    public static string nextAreaId = null;     //重刷场景名称
    public static int ShopType = 1;  //去的商店编号
    public static string CurrentPlayer = null;   //目前使用的是哪个存档

    public static bool suppressNextSelectSound = false;//二级菜单按钮选中声音吞掉
    public static bool suppressNextClickSound = false;//商店购买声音吞掉



    // 临时角色状态
    public static float? playerHealth = null;
    public static float? playerSex = null;


    //是否存在跨场景临时状态。 //null 表示从主菜单开始的新游戏。
    public static bool HasPlayerState =>playerHealth.HasValue;

    //开始新游戏时清空
    public static void ClearRunData()
    {
        nextAreaId = null;
        ShopType = 1;

        playerHealth = null;
        playerSex = null;
    }
}
