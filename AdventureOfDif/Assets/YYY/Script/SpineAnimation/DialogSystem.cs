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

    [Header("Dif")]
    public Image Dif_Image;
    public Sprite Dif_Happy, Dif_Angry, Dif_Sad, Dif_Confusion, Dif_Common;
    public GameObject Dif_Name;
    [Header("Enemy")]
    public Image Enemy_Image;
    public Sprite Enemy_01;
    public GameObject Enemy_Name;
    [Header("Boss")]
    public Image Boss_Image;
    public Sprite Boss_Common, Boss_Angry, Boss_Sad, Boss_Shame;
    public GameObject Boss_Name;
    [Header("Sin")]
    public Image Sin_Image;
    public Sprite Sin_Common, Sin_Happy, Sin_Confusion, Sin_Angry, Sin_Sad, Sin_Like;
    public GameObject Sin_Name;



    private void OnEnable()
    {
        //textLabel.text = textList[index];
        //index++;
        Invoke("Read", 0.1f);

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

        //switch (PlayerPrefs.GetInt("language"))
        //{
        //    case 0:
        //        textAssets.Add(1001, Resources.Load<TextAsset>("TXT_Japanese/J_Story_01"));
        //        break;
        //    case 1:
        //        textAssets.Add(1001, Resources.Load<TextAsset>("TXT_Simplified_Chinese/C1_Story_01"));
        //        break;
        //    case 2:
        //        textAssets.Add(1001, Resources.Load<TextAsset>("TXT_Traditional_Chinese/C2_Story_01"));
        //        break;
        //    case 3:
        //        textAssets.Add(1001, Resources.Load<TextAsset>("TXT_English/E_Story_01"));
        //        break;
        //    case 4:
        //        textAssets.Add(1001, Resources.Load<TextAsset>("TXT_Korean/K_Story_01"));                                                                                             
        //        break;
        //}


        textAssets.Add(1001, Resources.Load<TextAsset>("TXT_Simplified_Chinese/C1_Story_01"));



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



    IEnumerator SetTextUI()
    {

        //AudioManager.instance.AudioPlay(AudioManager.instance.Attack_pai1);

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





            #region [Dif]

            case "Dif_Happy":

                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(Dif)
                SetActiveSpeaker("Dif");
                Dif_Image.sprite = Dif_Happy;

                index++;
                break;

            case "Dif_Angry":

                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(Dif)
                SetActiveSpeaker("Dif");
                Dif_Image.sprite = Dif_Angry;

                index++;
                break;

            case "Dif_Sad":

                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(Dif)
                SetActiveSpeaker("Dif");
                Dif_Image.sprite = Dif_Angry;

                index++;
                break;

            case "Dif_Confusion":

                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(Dif)
                SetActiveSpeaker("Dif");
                Dif_Image.sprite = Dif_Confusion;

                index++;
                break;

            case "Dif_Common":

                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(Dif)
                SetActiveSpeaker("Dif");
                Dif_Image.sprite = Dif_Common;

                index++;
                break;
            #endregion

            #region [Sin]

            case "Sin_Common":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                Sin_Image.sprite = Sin_Common;

                index++;
                break;

            case "Sin_Happy":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                Sin_Image.sprite = Sin_Happy;

                index++;
                break;

            case "Sin_Confusion":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                Sin_Image.sprite = Sin_Confusion;

                index++;
                break;

            case "Sin_Angry":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                Sin_Image.sprite = Sin_Angry;

                index++;
                break;

            case "Sin_Sad":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                Sin_Image.sprite = Sin_Sad;

                index++;
                break;

            case "Sin_Like":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                Sin_Image.sprite = Sin_Like;

                index++;
                break;


            #endregion

            #region [Boss]

            case "Boss_Common":
                text.color = new Color(0.8f, 0.2f, 0.2f, 1.0f); // 深红色（女Boss）
                SetActiveSpeaker("Boss");
                Sin_Image.sprite = Boss_Common;

                index++;
                break;

            case "Boss_Angry":
                text.color = new Color(0.8f, 0.2f, 0.2f, 1.0f); // 深红色（女Boss）
                SetActiveSpeaker("Boss");
                Sin_Image.sprite = Boss_Angry;

                index++;
                break;

            case "Boss_Sad":
                text.color = new Color(0.8f, 0.2f, 0.2f, 1.0f); // 深红色（女Boss）
                SetActiveSpeaker("Boss");
                Sin_Image.sprite = Boss_Sad;

                index++;
                break;

            case "Boss_Shame":
                text.color = new Color(0.8f, 0.2f, 0.2f, 1.0f); // 深红色（女Boss）
                SetActiveSpeaker("Boss");
                Sin_Image.sprite = Boss_Shame;

                index++;
                break;

            #endregion

            #region [Enemy]

            case "Enemy_01":
                text.color = new Color(1.0f, 0.2f, 0.5f, 1.0f); //浅红色(Enemy)
                SetActiveSpeaker("Enemy");
                Enemy_Image.sprite = Enemy_01;

                index++;
                break;


                #endregion


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



    public void SetActiveSpeaker(string speaker)
    {
        // 先全部半黑 + 名字隐藏
        Color dark = new Color(0.5f, 0.5f, 0.5f, 1f);
        Color normal = Color.white;

        Dif_Image.color = dark;
        Boss_Image.color = dark;
        Enemy_Image.color = dark;
        Sin_Image.color = dark;

        Dif_Name.SetActive(false);
        Boss_Name.SetActive(false);
        Enemy_Name.SetActive(false);
        Sin_Name.SetActive(false);

        // 根据说话人激活对应角色与名字，并还原颜色
        switch (speaker)
        {
            case "Dif":
                Dif_Image.color = normal;
                Dif_Name.SetActive(true);
                Dif_Image.gameObject.SetActive(true);
                break;

            case "Boss":
                Boss_Image.color = normal;
                Boss_Name.SetActive(true);
                Boss_Image.gameObject.SetActive(true);

                //Boss出现，Sin消失
                Sin_Image.gameObject.SetActive(false);
                break;

            case "Enemy":
                Enemy_Image.color = normal;
                Enemy_Name.SetActive(true);
                Enemy_Image.gameObject.SetActive(true);
                break;

            case "Sin":
                Sin_Image.color = normal;
                Sin_Name.SetActive(true);
                Sin_Image.gameObject.SetActive(true);

                //Sin出现，Boss消失
                Boss_Image.gameObject.SetActive(false);
                break;
        }
    }



    //快进按钮触发在这里
    public void ChangeStory()
    {

        UIManager.instance.CloseAVG();
        

        //AudioManager.instance.AudioPlay(AudioManager.instance.Attack_hit2);

        gameObject.SetActive(false);
    }




}
