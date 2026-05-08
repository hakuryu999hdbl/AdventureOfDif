using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceManager : MonoBehaviour
{
    public static BalanceManager instance { get; private set; }
    void Awake()
    {
        instance = this;
    }


    [Header("金币")]
    public Text MoneyText;

    private void Start()
    {
        ChangeMoney(0, false);//更新钱
    }


    public void ChangeMoney(int amount, bool UseVoice = true)
    {
        // 取当前值
        SaveData _data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);
        int currentMoney = _data.Money;


        // 修改
        currentMoney += amount;
        if (currentMoney < 0) currentMoney = 0;   // 防止出现负数

        // ✅ 把修改结果写回存档对象
        _data.Money = currentMoney;
        // ✅ 保存回文件
        SaveManager.SaveGame(_data);

        // 更新 UI
        MoneyText.text = currentMoney.ToString();


        //if (UseVoice) { AudioManager.instance.AudioPlay(AudioManager.instance.SE_Reji); }

    }
}
