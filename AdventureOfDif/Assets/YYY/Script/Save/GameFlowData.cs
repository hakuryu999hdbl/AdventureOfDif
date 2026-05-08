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
}
