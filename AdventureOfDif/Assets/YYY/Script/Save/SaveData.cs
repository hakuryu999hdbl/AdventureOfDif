using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string slotName = "CurrentPlayer"; //存档名称名字
    public string saveTime;// 存档时间（字符串）
    public string NextAreaId;//当前区域位置(进入该场景时保存)


    public int Money;

    public int Pudding;
    public int Cola;
    public int ChocoBanana;
    public int Cone;
    public int CreamRoll;
    public int FriedCutlet;
    public int Potion;
    public int Incense;
    public int ButtPlug;
    public int Vibrator;

    public int GetItem(string key)
    {
        switch (key)
        {
            case "Pudding": return Pudding;
            case "Cola": return Cola;
            case "ChocoBanana": return ChocoBanana;
            case "Cone": return Cone;
            case "CreamRoll": return CreamRoll;
            case "FriedCutlet": return FriedCutlet;
            case "Potion": return Potion;
            case "Incense": return Incense;
            case "ButtPlug": return ButtPlug;
            case "Vibrator": return Vibrator;
            default: return 0;
        }
    }
    public void SetItem(string key, int value)
    {
        switch (key)
        {
            case "Pudding": Pudding = value; break;
            case "Cola": Cola = value; break;
            case "ChocoBanana": ChocoBanana = value; break;
            case "Cone": Cone = value; break;
            case "CreamRoll": CreamRoll = value; break;
            case "FriedCutlet": FriedCutlet = value; break;
            case "Potion": Potion = value; break;
            case "Incense": Incense = value; break;
            case "ButtPlug": ButtPlug = value; break;
            case "Vibrator": Vibrator = value; break;
        }
    }






    public void AddItem(string key, int delta)
    {
        int newValue = Mathf.Max(0, GetItem(key) + delta);
        SetItem(key, newValue);
    }





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
