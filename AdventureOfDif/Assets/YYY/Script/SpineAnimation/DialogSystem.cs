using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [Header("UI组件")]
    public Text textLabel;

    private Dictionary<int, TextAsset> textAssets = new Dictionary<int, TextAsset>();


    public int index;
    public float textSpeed;
    bool textFinished;//是否完成打字
    bool cancelTyping;//取消打字
    List<string> textList = new List<string>();


    [Header("动画控制器")]
    public int animation_number;

    [Header("对话，背景，角色")]
    public GameObject TextButton;

    public Image BG_Image;
    public Sprite Story_00, 
                  Story_01, Story_02, Story_03, Story_04, Story_05, Story_06, Story_07, Story_08, Story_09, Story_10,
                  Story_11, Story_12, Story_13, Story_14, Story_15, Story_16, Story_17, Story_18, Story_19, Story_20, 
                  Story_21, Story_22, Story_23, Story_24, Story_25, Story_26, Story_27, Story_28,
                  Story_29, Story_30, Story_31, Story_32, Story_33, Story_34,
                  Story_35, Story_36, Story_37, Story_38, Story_39, Story_40,
                  Story_41, Story_42, Story_43, Story_44, Story_45, Story_46, Story_47, Story_48, Story_49,
                  Story_50, Story_51, Story_52, Story_53, Story_54, Story_55, Story_56, Story_57, Story_58, Story_59, Story_60, Story_61, Story_62, Story_63, Story_64,
                  Story_65, Story_66, Story_67, Story_68, Story_69, Story_70, Story_71, Story_72, Story_73, Story_74, Story_75, Story_76, Story_77,
                  Story_78, Story_79, Story_80, Story_81, Story_82, Story_83, Story_84,
                  Story_85, Story_86, Story_87, Story_88, Story_89, Story_90;


    private void OnEnable()
    {
        //textLabel.text = textList[index];
        //index++;
        Invoke("Read",0.1f);

    }//一开始不会产生空白，OnEnable会在Start之前，Awake之后被调用


    public void ForceEndDialogue()
    {
        // 清除当前对话状态
        textList.Clear();
        index = 0;

        // 设置 textFinished 为 true，以便退出正在进行的协程
        textFinished = true;

        // 将对话系统 UI 隐藏
        gameObject.SetActive(false);

        Debug.Log("对话已强制结束并重置");


    }//强制关闭对话

    void Read()
    {
        // Clear the existing dictionary to avoid key conflicts
        textAssets.Clear();

        switch (PlayerPrefs.GetInt("language"))
        {
            case 0:
                textAssets.Add(999, Resources.Load<TextAsset>("TXT_Japanese/J_AI_Animation_Start"));

                textAssets.Add(1001, Resources.Load<TextAsset>("TXT_Japanese/J_Story_1"));
                textAssets.Add(1002, Resources.Load<TextAsset>("TXT_Japanese/J_Story_2"));
                textAssets.Add(1003, Resources.Load<TextAsset>("TXT_Japanese/J_Story_3"));
                textAssets.Add(1004, Resources.Load<TextAsset>("TXT_Japanese/J_Story_4"));
                textAssets.Add(1005, Resources.Load<TextAsset>("TXT_Japanese/J_Story_5"));
                textAssets.Add(1006, Resources.Load<TextAsset>("TXT_Japanese/J_Story_6"));
                textAssets.Add(1007, Resources.Load<TextAsset>("TXT_Japanese/J_Story_7"));
                textAssets.Add(1008, Resources.Load<TextAsset>("TXT_Japanese/J_Story_8"));
                textAssets.Add(1009, Resources.Load<TextAsset>("TXT_Japanese/J_Story_9"));
                break;
            case 1:
                textAssets.Add(999, Resources.Load<TextAsset>("TXT_Chinese/C_AI_Animation_Start"));

                textAssets.Add(1001, Resources.Load<TextAsset>("TXT_Chinese/C_Story_1"));
                textAssets.Add(1002, Resources.Load<TextAsset>("TXT_Chinese/C_Story_2"));
                textAssets.Add(1003, Resources.Load<TextAsset>("TXT_Chinese/C_Story_3"));
                textAssets.Add(1004, Resources.Load<TextAsset>("TXT_Chinese/C_Story_4"));
                textAssets.Add(1005, Resources.Load<TextAsset>("TXT_Chinese/C_Story_5"));
                textAssets.Add(1006, Resources.Load<TextAsset>("TXT_Chinese/C_Story_6"));
                textAssets.Add(1007, Resources.Load<TextAsset>("TXT_Chinese/C_Story_7"));
                textAssets.Add(1008, Resources.Load<TextAsset>("TXT_Chinese/C_Story_8"));
                textAssets.Add(1009, Resources.Load<TextAsset>("TXT_Chinese/C_Story_9"));
                break;
            case 2:
                textAssets.Add(999, Resources.Load<TextAsset>("TXT_English/E_AI_Animation_Start"));

                textAssets.Add(1001, Resources.Load<TextAsset>("TXT_English/E_Story_1"));
                textAssets.Add(1002, Resources.Load<TextAsset>("TXT_English/E_Story_2"));
                textAssets.Add(1003, Resources.Load<TextAsset>("TXT_English/E_Story_3"));
                textAssets.Add(1004, Resources.Load<TextAsset>("TXT_English/E_Story_4"));
                textAssets.Add(1005, Resources.Load<TextAsset>("TXT_English/E_Story_5"));
                textAssets.Add(1006, Resources.Load<TextAsset>("TXT_English/E_Story_6"));
                textAssets.Add(1007, Resources.Load<TextAsset>("TXT_English/E_Story_7"));
                textAssets.Add(1008, Resources.Load<TextAsset>("TXT_English/E_Story_8"));
                textAssets.Add(1009, Resources.Load<TextAsset>("TXT_English/E_Story_9"));
                break;
        }

      




        // 使用字典查找相应的 TextAsset
        if (textAssets.TryGetValue(animation_number, out TextAsset selectedText))
        {
            GetTextFormFile(selectedText);
        }
        else
        {
            Debug.LogError("No TextAsset found for animation_number: " + animation_number);
        }

        textFinished = true;
        StartCoroutine(SetTextUI());
    }

    public void ShowText()
    {
        if (textFinished && !cancelTyping)
        {
            if (index >= textList.Count) // 添加边界检查
            {
                gameObject.SetActive(false);
                index = 0;

                ChangeStory();//结束重刷场景

                Debug.Log("对话已结束");
                return;
            }

            if (gameObject.activeSelf)
            {
                StartCoroutine(SetTextUI());
            }
        }
        else if (!textFinished)
        {
            cancelTyping = !cancelTyping;
        }

    }

    void GetTextFormFile(TextAsset file)
    {
        textList.Clear(); index = 0;//首先将列表内的字符清空

        var lineDate = file.text.Split('\n');//以回车切割每一段

        foreach (var line in lineDate)
        {
            textList.Add(line);
        }
    }

    [Header("结局动画中有没有邻过奖")]
    public bool isTakePrize = false;


    IEnumerator SetTextUI()
    {
        if (index >= textList.Count)
        {
            Debug.LogWarning("index 超出 textList 范围");
            yield break;
        }

        textFinished = false;
        textLabel.text = "";

        //判断一整行的字符是
        Text text = textLabel;
        switch (textList[index].Trim().ToString())
        {
            //字的颜色
            case "BG":
                text.color = Color.white;
                index++;
                break;





            //case "Girl":
            //    text.color = new Color(1.0f, 0.0f, 1.0f, 1.0f);//粉色
            //    index++;
            //    break;

            case "MAN":
                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(市民群众)
                index++;
                break;
            case "DarkRed":
                text.color = new Color(0.8f, 0.2f, 0.2f, 1.0f); // 深红色(梦魔)（女特工）
                index++;
                break;
            case "LightRed":
                text.color = new Color(1.0f, 0.2f, 0.5f, 1.0f); //浅红色(菲西莉亚)
                index++;
                break;
            case "Green":
                text.color = new Color(0.0f, 1.0f, 0.0f, 1.0f); // 绿色（魔族女干部）
                index++;
                break;
            case "LightBlue":
                text.color = new Color(0.68f, 0.85f, 0.9f, 1.0f); // 浅蓝色（艾莉丝）
                index++;
                break;
            case "Gold":
                text.color = new Color(1.0f, 0.84f, 0.0f, 1.0f); // 金色（叛变战姬大队长）
                index++;
                break;
            case "Yellow":
                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（莱拉）
                index++;
                break;
            case "Orange":
                text.color = new Color(1.0f, 0.5f, 0.0f, 1.0f); // 橙色(播种母体)
                index++;
                break;
            case "Purple":
                text.color = new Color(0.7f, 0.3f, 0.7f, 1.0f); // 紫色 (女记者)
                index++;
                break;
            case "Gray":
                text.color = new Color(0.7f, 0.75f, 0.8f, 1.0f); // 亮灰色(牧者)（政府特工）(研究员)
                index++;
                break;



            //case "Over":
            //    ChangeStory();//通常对话结束
            //    index++;
            //    break;
            //
            //
            //case "ReStart":
            //    //Spine_FrameEvents.ReStart();//教程结束回主菜单
            //    index++;
            //    break;

         


            //case "CleanStage":
            //    CleanStage();//通关重置
            //    index++;
            //    break;
               


            case "Story_00":
                BG_Image.sprite = Story_00;
                text.color = Color.white;
                index++;
                break;
            case "Story_01":
                BG_Image.sprite = Story_01;
                text.color = Color.white;
                index++;
                break;
            case "Story_02":
                BG_Image.sprite = Story_02;
                text.color = Color.white;
                index++;
                break;
            case "Story_03":
                BG_Image.sprite = Story_03;
                text.color = Color.white;
                index++;
                break;
            case "Story_04":
                BG_Image.sprite = Story_04;
                text.color = Color.white;
                index++;
                break;
            case "Story_05":
                BG_Image.sprite = Story_05;
                text.color = Color.white;
                index++;
                break;
            case "Story_06":
                BG_Image.sprite = Story_06;
                text.color = Color.white;
                index++;
                break;
            case "Story_07":
                BG_Image.sprite = Story_07;
                text.color = Color.white;
                index++;
                break;
            case "Story_08":
                BG_Image.sprite = Story_08;
                text.color = Color.white;
                index++;
                break;
            case "Story_09":
                BG_Image.sprite = Story_09;
                text.color = Color.white;
                index++;
                break;
            case "Story_10":
                BG_Image.sprite = Story_10;
                text.color = Color.white;
                index++;
                break;
            case "Story_11":
                BG_Image.sprite = Story_11;
                text.color = Color.white;
                index++;
                break;
            case "Story_12":
                BG_Image.sprite = Story_12;
                text.color = Color.white;
                index++;
                break;
            case "Story_13":
                BG_Image.sprite = Story_13;
                text.color = Color.white;
                index++;
                break;
            case "Story_14":
                BG_Image.sprite = Story_14;
                text.color = Color.white;
                index++;
                break;
            case "Story_15":
                BG_Image.sprite = Story_15;
                text.color = Color.white;
                index++;
                break;
            case "Story_16":
                BG_Image.sprite = Story_16;
                text.color = Color.white;
                index++;
                break;
            case "Story_17":
                BG_Image.sprite = Story_17;
                text.color = Color.white;
                index++;
                break;
            case "Story_18":
                BG_Image.sprite = Story_18;
                text.color = Color.white;
                index++;
                break;
            case "Story_19":
                BG_Image.sprite = Story_19;
                text.color = Color.white;
                index++;
                break;
            case "Story_20":
                BG_Image.sprite = Story_20;
                text.color = Color.white;
                index++;
                break;
            case "Story_21":
                BG_Image.sprite = Story_21;
                text.color = Color.white;
                index++;
                break;
            case "Story_22":
                BG_Image.sprite = Story_22;
                text.color = Color.white;
                index++;
                break;
            case "Story_23":
                BG_Image.sprite = Story_23;
                text.color = Color.white;
                index++;
                break;
            case "Story_24":
                BG_Image.sprite = Story_24;
                text.color = Color.white;
                index++;
                break;
            case "Story_25":
                BG_Image.sprite = Story_25;
                text.color = Color.white;
                index++;
                break;
            case "Story_26":
                BG_Image.sprite = Story_26;
                text.color = Color.white;
                index++;
                break;
            case "Story_27":
                BG_Image.sprite = Story_27;
                text.color = Color.white;
                index++;
                break;
            case "Story_28":
                BG_Image.sprite = Story_28;
                text.color = Color.white;
                index++;
                break;
            case "Story_29":
                BG_Image.sprite = Story_29;
                text.color = Color.white;
                index++;
                break;
            case "Story_30":
                BG_Image.sprite = Story_30;
                text.color = Color.white;
                index++;
                break;
            case "Story_31":
                BG_Image.sprite = Story_31;
                text.color = Color.white;
                index++;
                break;
            case "Story_32":
                BG_Image.sprite = Story_32;
                text.color = Color.white;
                index++;
                break;
            case "Story_33":
                BG_Image.sprite = Story_33;
                text.color = Color.white;
                index++;
                break;
            case "Story_34":
                BG_Image.sprite = Story_34;
                text.color = Color.white;
                index++;
                break;
            case "Story_35":
                BG_Image.sprite = Story_35;
                text.color = Color.white;
                index++;
                break;
            case "Story_36":
                BG_Image.sprite = Story_36;
                text.color = Color.white;
                index++;
                break;
            case "Story_37":
                BG_Image.sprite = Story_37;
                text.color = Color.white;
                index++;
                break;
            case "Story_38":
                BG_Image.sprite = Story_38;
                text.color = Color.white;
                index++;
                break;
            case "Story_39":
                BG_Image.sprite = Story_39;
                text.color = Color.white;
                index++;
                break;
            case "Story_40":
                BG_Image.sprite = Story_40;
                text.color = Color.white;
                index++;
                break;
            case "Story_41":
                BG_Image.sprite = Story_41;
                text.color = Color.white;
                index++;
                break;
            case "Story_42":
                BG_Image.sprite = Story_42;
                text.color = Color.white;
                index++;
                break;
            case "Story_43":
                BG_Image.sprite = Story_43;
                text.color = Color.white;
                index++;
                break;
            case "Story_44":
                BG_Image.sprite = Story_44;
                text.color = Color.white;
                index++;
                break;
            case "Story_45":
                BG_Image.sprite = Story_45;
                text.color = Color.white;
                index++;
                break;
            case "Story_46":
                BG_Image.sprite = Story_46;
                text.color = Color.white;
                index++;
                break;
            case "Story_47":
                BG_Image.sprite = Story_47;
                text.color = Color.white;
                index++;
                break;
            case "Story_48":
                BG_Image.sprite = Story_48;
                text.color = Color.white;
                index++;
                break;
            case "Story_49":
                BG_Image.sprite = Story_49;
                text.color = Color.white;
                index++;
                break;
            case "Story_50":
                BG_Image.sprite = Story_50;
                text.color = Color.white;
                index++;
                break;
            case "Story_51":
                BG_Image.sprite = Story_51;
                text.color = Color.white;
                index++;
                break;
            case "Story_52":
                BG_Image.sprite = Story_52;
                text.color = Color.white;
                index++;
                break;
            case "Story_53":
                BG_Image.sprite = Story_53;
                text.color = Color.white;
                index++;
                break;
            case "Story_54":
                BG_Image.sprite = Story_54;
                text.color = Color.white;
                index++;
                break;
            case "Story_55":
                BG_Image.sprite = Story_55;
                text.color = Color.white;
                index++;
                break;
            case "Story_56":
                BG_Image.sprite = Story_56;
                text.color = Color.white;
                index++;
                break;
            case "Story_57":
                BG_Image.sprite = Story_57;
                text.color = Color.white;
                index++;
                break;
            case "Story_58":
                BG_Image.sprite = Story_58;
                text.color = Color.white;
                index++;
                break;
            case "Story_59":
                BG_Image.sprite = Story_59;
                text.color = Color.white;
                index++;
                break;
            case "Story_60":
                BG_Image.sprite = Story_60;
                text.color = Color.white;
                index++;
                break;
            case "Story_61":
                BG_Image.sprite = Story_61;
                text.color = Color.white;
                index++;
                break;
            case "Story_62":
                BG_Image.sprite = Story_62;
                text.color = Color.white;
                index++;
                break;
            case "Story_63":
                BG_Image.sprite = Story_63;
                text.color = Color.white;
                index++;
                break;
            case "Story_64":
                BG_Image.sprite = Story_64;
                text.color = Color.white;
                index++;
                break;
            case "Story_65":
                BG_Image.sprite = Story_65;
                text.color = Color.white;
                index++;
                break;
            case "Story_66":
                BG_Image.sprite = Story_66;
                text.color = Color.white;
                index++;
                break;
            case "Story_67":
                BG_Image.sprite = Story_67;
                text.color = Color.white;
                index++;
                break;
            case "Story_68":
                BG_Image.sprite = Story_68;
                text.color = Color.white;
                index++;
                break;
            case "Story_69":
                BG_Image.sprite = Story_69;
                text.color = Color.white;
                index++;
                break;
            case "Story_70":
                BG_Image.sprite = Story_70;
                text.color = Color.white;
                index++;
                break;
            case "Story_71":
                BG_Image.sprite = Story_71;
                text.color = Color.white;
                index++;
                break;
            case "Story_72":
                BG_Image.sprite = Story_72;
                text.color = Color.white;
                index++;
                break;
            case "Story_73":
                BG_Image.sprite = Story_73;
                text.color = Color.white;
                index++;
                break;
            case "Story_74":
                BG_Image.sprite = Story_74;
                text.color = Color.white;
                index++;
                break;
            case "Story_75":
                BG_Image.sprite = Story_75;
                text.color = Color.white;
                index++;
                break;
            case "Story_76":
                BG_Image.sprite = Story_76;
                text.color = Color.white;
                index++;
                break;
            case "Story_77":
                BG_Image.sprite = Story_77;
                text.color = Color.white;
                index++;
                break;
            case "Story_78":
                BG_Image.sprite = Story_78;
                text.color = Color.white;
                index++;
                break;
            case "Story_79":
                BG_Image.sprite = Story_79;
                text.color = Color.white;
                index++;
                break;
            case "Story_80":
                BG_Image.sprite = Story_80;
                text.color = Color.white;
                index++;
                break;
            case "Story_81":
                BG_Image.sprite = Story_81;
                text.color = Color.white;
                index++;
                break;
            case "Story_82":
                BG_Image.sprite = Story_82;
                text.color = Color.white;
                index++;
                break;
            case "Story_83":
                BG_Image.sprite = Story_83;
                text.color = Color.white;
                index++;
                break;
            case "Story_84":
                BG_Image.sprite = Story_84;
                text.color = Color.white;
                index++;
                break;
            case "Story_85":
                BG_Image.sprite = Story_85;
                text.color = Color.white;
                index++;
                break;
            case "Story_86":
                BG_Image.sprite = Story_86;
                text.color = Color.white;
                index++;
                break;
            case "Story_87":
                BG_Image.sprite = Story_87;
                text.color = Color.white;
                index++;
                break;
            case "Story_88":
                BG_Image.sprite = Story_88;
                text.color = Color.white;
                index++;
                break;
            case "Story_89":
                BG_Image.sprite = Story_89;
                text.color = Color.white;
                index++;
                break;
            case "Story_90":
                BG_Image.sprite = Story_90;
                text.color = Color.white;
                index++;
                break;
        }


        int letter = 0;
        while (!cancelTyping && letter < textList[index].Length - 1)
        {
            textLabel.text += textList[index][letter];
            letter++;
            yield return new WaitForSeconds(textSpeed);
        }

        textLabel.text = textList[index];
        cancelTyping = false;
        textFinished = true;
        index++;
    }


    //快进按钮触发在这里
    public void ChangeStory()
    {
        
       
      


    }




}
