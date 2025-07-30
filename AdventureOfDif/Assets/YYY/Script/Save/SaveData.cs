using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public string slotName = "CurrentPlayer"; //存档名称名字
    public string saveTime;// 存档时间（字符串）
    public string NextAreaId;//当前区域位置(进入该场景时保存)

    // ✅ 加上这个构造函数 ↓↓↓↓↓↓↓↓↓
    public SaveData(string name)
    {
        slotName = name;
        saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    // ✅ 如果你也调用过 new SaveData() 这种无参数形式，也要保留这个：
    public SaveData()
    {
        saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
