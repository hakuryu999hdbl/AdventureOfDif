using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Linq;

public static class SaveManager
{
    private static string GetPath(string saveName)
    {
        return Application.persistentDataPath + "/" + saveName + ".json";
    }

    public static void SaveGame(SaveData data)
    {
        data.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // 更新时间
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(data.slotName), json);
        //Debug.Log("保存到：" + GetPath(data.slotName));
    }//储存存档

    public static SaveData LoadGame(string saveName)
    {
        string path = GetPath(saveName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.Log("未找到存档：" + saveName);
            return new SaveData(saveName);
        }
    }//读取存档

    public static void DeleteGame(string saveName)
    {
        string path = GetPath(saveName);
        if (File.Exists(path)) File.Delete(path);
    }//删除此存档

    public static bool Exists(string saveName)
    {
        return File.Exists(GetPath(saveName));
    }//确认这个存档是否存在
}