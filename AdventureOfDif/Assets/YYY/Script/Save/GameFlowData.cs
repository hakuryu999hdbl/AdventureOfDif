using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameFlowData 
{
    public static string nextAreaId = null;     //重刷场景名称
    public static string CurrentPlayer = null;   //目前使用的是哪个存档
    public static bool suppressNextSelectSound = false;//二级菜单按钮选中声音吞掉
}
