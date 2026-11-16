using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour
{
    [Header("UI组件")]
    public Text textLabel;

    private static readonly Dictionary<int, string> DialogueFiles = new Dictionary<int, string>
    {
        [101] = "01_01",
        [201] = "02_01",
        [202] = "02_02",
        [203] = "02_03",
        [204] = "02_03_A",
        [205] = "02_03_B",
        [206] = "02_03_C",
        [207] = "02_03_D",
        [208] = "02_04",
        [209] = "02_05",
        [301] = "03_01",
        [302] = "03_02",
        [401] = "04_01",
        [402] = "04_02_A",
        [403] = "04_02_B",
        [404] = "04_02_C",
        [405] = "04_02_D",
        [501] = "05_01",
        [502] = "05_02",
        [503] = "05_03_LOSE",
        [504] = "05_03_WIN",
        [505] = "05_04",
        [601] = "06_01",
        [602] = "06_02",
        [603] = "06_03",
        [604] = "06_04",
    };


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
    public Sprite Dif_Angry, Dif_Doubt, Dif_Happy, Dif_Normal, Dif_Shy, Dif_Surprise;
    public GameObject Dif_Name;
    [Header("Enemy_1")]
    public Image Enemy_1_Image;
    public Sprite Enemy_1_Hit, Enemy_1_Normal;
    public GameObject Enemy_1_Name;
    [Header("Scarlett")]
    public Image Scarlett_Image;
    public Sprite Scarlett_Angry, Scarlett_Defeat, Scarlett_Happy, Scarlett_Normal, Scarlett_Shy;
    public GameObject Scarlett_Name;
    [Header("Sin")]
    public Image Sin_Image;
    [SerializeField] private Image sinLeftImage;
    public Sprite Sin_Angry, Sin_Doubt, Sin_Happy, Sin_Normal, Sin_Sad, Sin_Shy;
    public GameObject Sin_Name;

    private Image CurrentSinImage =>
        sinLeftImage != null && ShouldUseLeftSin(animation_number) ? sinLeftImage : Sin_Image;

    private static bool ShouldUseLeftSin(int dialogueNumber)
    {
        // 需要让 Sin 站在 Dif 旁边的对话编号。
        return dialogueNumber is 208 or 209 or 301
            or 502 or 505 or 604;
    }
    [Header("Leo")]
    public Image Leo_Image;
    public Sprite Leo_Doubt, Leo_Happy, Leo_Normal;
    public GameObject Leo_Name;
    [Header("Lanz")]
    public Image Lanz_Image;
    public Sprite Lanz_Focus, Lanz_Normal;
    public GameObject Lanz_Name;
    [Header("YYY")]
    public Image YYY_Image;
    public Sprite YYY_Happy, YYY_Normal;
    public GameObject YYY_Name;

    private void OnEnable()
    {
        if (avgChoice != null)
            avgChoice.ResetChoices();

        //textLabel.text = textList[index];
        //index++;
        Invoke("Read", 0.1f);

        HideAll();

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

    private static (string folder, string prefix) GetDialogueLocale(int language)
    {
        return language switch
        {
            0 => ("TXT_Japanese", "J"),
            2 => ("TXT_Traditional_Chinese", "C2"),
            3 => ("TXT_English", "E"),
            4 => ("TXT_Korean", "K"),
            _ => ("TXT_Simplified_Chinese", "C1")
        };
    }

    void Read()
    {
        if (!DialogueFiles.TryGetValue(animation_number, out string file))
        {
            Debug.LogError($"No dialogue file mapped for animation_number: {animation_number}", this);
            ChangeStory();
            return;
        }

        var (folder, prefix) = GetDialogueLocale(PlayerPrefs.GetInt("language", 0));
        TextAsset selectedText = Resources.Load<TextAsset>($"{folder}/{prefix}_{file}");
        if (selectedText == null && prefix != "C1")
        {
            Debug.LogWarning($"Missing localized dialogue {folder}/{prefix}_{file}; using Simplified Chinese.", this);
            selectedText = Resources.Load<TextAsset>($"TXT_Simplified_Chinese/C1_{file}");
        }

        if (selectedText == null)
        {
            Debug.LogError($"No text asset for dialogue {animation_number} ({file})", this);
            UIManager.instance.CloseAVG();
            gameObject.SetActive(false);
            return;
        }

        GetTextFormFile(selectedText);
        textFinished = true;
        StartCoroutine(SetTextUI());
    }

    public void ShowText()
    {
        if (avgChoice != null && avgChoice.isActiveAndEnabled)
            return;

        if (textFinished && !cancelTyping)
        {
            if (index >= textList.Count) // 添加边界检查
            {
              
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
        string speakerTag = textList[index].Trim();
        switch (speakerTag)
        {
            //字的颜色
            case "bg":
                text.color = Color.white;
                HideAll();

                index++;
                break;





            #region [Dif]

            case "dif_angry":

                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(Dif)
                SetActiveSpeaker("Dif");
                Dif_Image.sprite = Dif_Angry;

                index++;
                break;

            case "dif_doubt":

                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(Dif)
                SetActiveSpeaker("Dif");
                Dif_Image.sprite = Dif_Doubt;

                index++;
                break;

            case "dif_happy":

                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(Dif)
                SetActiveSpeaker("Dif");
                Dif_Image.sprite = Dif_Happy;

                index++;
                break;

            case "dif_normal":

                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(Dif)
                SetActiveSpeaker("Dif");
                Dif_Image.sprite = Dif_Normal;

                index++;
                break;

            case "dif_shy":

                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(Dif)
                SetActiveSpeaker("Dif");
                Dif_Image.sprite = Dif_Shy;

                index++;
                break;

            case "dif_surprise":

                text.color = new Color(0.0f, 0.68f, 0.93f, 1.0f);//蓝色(Dif)
                SetActiveSpeaker("Dif");
                Dif_Image.sprite = Dif_Surprise;

                index++;
                break;
            #endregion

            #region [Sin]

            case "sin_angry":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                CurrentSinImage.sprite = Sin_Angry;

                index++;
                break;

            case "sin_doubt":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                CurrentSinImage.sprite = Sin_Doubt;

                index++;
                break;

            case "sin_happy":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                CurrentSinImage.sprite = Sin_Happy;

                index++;
                break;

            case "sin_normal":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                CurrentSinImage.sprite = Sin_Normal;

                index++;
                break;

            case "sin_sad":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                CurrentSinImage.sprite = Sin_Sad;

                index++;
                break;

            case "sin_shy":

                text.color = new Color(1.0f, 1.0f, 0.0f, 1.0f); // 黄色（Sin）
                SetActiveSpeaker("Sin");
                CurrentSinImage.sprite = Sin_Shy;

                index++;
                break;


            #endregion

            #region [Scarlett]

            case "scarlett_angry":
                text.color = new Color(0.8f, 0.2f, 0.2f, 1.0f); // 深红色（女Boss）
                SetActiveSpeaker("Scarlett");
                Sin_Image.sprite = Scarlett_Angry;

                index++;
                break;

            case "scarlett_defeat":
                text.color = new Color(0.8f, 0.2f, 0.2f, 1.0f); // 深红色（女Boss）
                SetActiveSpeaker("Scarlett");
                Sin_Image.sprite = Scarlett_Defeat;

                index++;
                break;

            case "scarlett_happy":
                text.color = new Color(0.8f, 0.2f, 0.2f, 1.0f); // 深红色（女Boss）
                SetActiveSpeaker("Scarlett");
                Sin_Image.sprite = Scarlett_Happy;

                index++;
                break;

            case "scarlett_normal":
                text.color = new Color(0.8f, 0.2f, 0.2f, 1.0f); // 深红色（女Boss）
                SetActiveSpeaker("Scarlett");
                Sin_Image.sprite = Scarlett_Normal;

                index++;
                break;

            case "scarlett_shy":
                text.color = new Color(0.8f, 0.2f, 0.2f, 1.0f); // 深红色（女Boss）
                SetActiveSpeaker("Scarlett");
                Sin_Image.sprite = Scarlett_Shy;

                index++;
                break;

            #endregion

            #region [Enemy_1]

            case "enemy_hit":
            case "enemy1_hit":
                text.color = new Color(1.0f, 0.2f, 0.5f, 1.0f); //浅红色(Enemy)
                SetActiveSpeaker("Enemy");
                Enemy_1_Image.sprite = Enemy_1_Hit;

                index++;
                break;

            case "enemy1_normal":
                text.color = new Color(1.0f, 0.2f, 0.5f, 1.0f); //浅红色(Enemy)
                SetActiveSpeaker("Enemy");
                Enemy_1_Image.sprite = Enemy_1_Normal;

                index++;
                break;

            #endregion

            #region [Leo]

            case "leo_doubt":
                text.color = new Color(0.71f, 0.94f, 0.28f, 1f); //青柠色(Leo)
                SetActiveSpeaker("Leo");
                Leo_Image.sprite = Leo_Doubt;

                index++;
                break;

            case "leo_happy":
                text.color = new Color(0.71f, 0.94f, 0.28f, 1f); //青柠色(Leo)
                SetActiveSpeaker("Leo");
                Leo_Image.sprite = Leo_Happy;

                index++;
                break;

            case "leo_normal":
                text.color = new Color(0.71f, 0.94f, 0.28f, 1f); //青柠色(Leo)
                SetActiveSpeaker("Leo");
                Leo_Image.sprite = Leo_Normal;

                index++;
                break;

            #endregion

            #region [Lanz]

            case "lanz_focus":
            case "lanz_normal":
                text.color = new Color(0.78f, 0.42f, 0.33f, 1f); //棕红色(Lanz)
                SetActiveSpeaker("Lanz");
                Lanz_Image.sprite = speakerTag == "lanz_focus" ? Lanz_Focus : Lanz_Normal;

                index++;
                break;
            #endregion

            #region [YYY]

            case "queque_happy":
                text.color = new Color(1.00f, 0.50f, 0.74f, 1f);//粉色（Pink）
                SetActiveSpeaker("YYY");
                YYY_Image.sprite = YYY_Happy;

                index++;
                break;

            case "queque_normal":
                text.color = new Color(1.00f, 0.50f, 0.74f, 1f);//粉色（Pink）
                SetActiveSpeaker("YYY");
                YYY_Image.sprite = YYY_Normal;

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

    void HideAll()
    {
        Dif_Name.SetActive(false);
        Scarlett_Name.SetActive(false);
        Enemy_1_Name.SetActive(false);
        Sin_Name.SetActive(false);
        Leo_Name.SetActive(false);
        Lanz_Name.SetActive(false);
        YYY_Name.SetActive(false);


        Dif_Image.gameObject.SetActive(false);
        Scarlett_Image.gameObject.SetActive(false);
        Enemy_1_Image.gameObject.SetActive(false);
        Sin_Image.gameObject.SetActive(false);
        if (sinLeftImage != null)
            sinLeftImage.gameObject.SetActive(false);
        Leo_Image.gameObject.SetActive(false);
        Lanz_Image.gameObject.SetActive(false);
        YYY_Image.gameObject.SetActive(false);
    }

    public void SetActiveSpeaker(string speaker)
    {
        // 先全部半黑 + 名字隐藏
        Color dark = new Color(0.5f, 0.5f, 0.5f, 1f);
        Color normal = Color.white;

        Dif_Image.color = dark;
        Scarlett_Image.color = dark;
        Enemy_1_Image.color = dark;
        CurrentSinImage.color = dark;
        Leo_Image.color = dark;
        Lanz_Image.color = dark;
        YYY_Image.color = dark;



        Dif_Name.SetActive(false);
        Scarlett_Name.SetActive(false);
        Enemy_1_Name.SetActive(false);
        Sin_Name.SetActive(false);
        Leo_Name.SetActive(false);
        Lanz_Name.SetActive(false);
        YYY_Name.SetActive(false);

        // 根据说话人激活对应角色与名字，并还原颜色
        switch (speaker)
        {
            case "Dif":
                Dif_Image.color = normal;
                Dif_Name.SetActive(true);
                Dif_Image.gameObject.SetActive(true);
                break;

            case "Scarlett":
                Scarlett_Image.color = normal;
                Scarlett_Name.SetActive(true);
                Scarlett_Image.gameObject.SetActive(true);
                break;

            case "Enemy":
                Enemy_1_Image.color = normal;
                Enemy_1_Name.SetActive(true);
                Enemy_1_Image.gameObject.SetActive(true);
                break;

            case "Sin":
                Image sinPortrait = CurrentSinImage;
                Sin_Image.gameObject.SetActive(sinPortrait == Sin_Image);
                if (sinLeftImage != null)
                    sinLeftImage.gameObject.SetActive(sinPortrait == sinLeftImage);
                sinPortrait.color = normal;
                Sin_Name.SetActive(true);
                break;

            case "Leo":
                Leo_Image.color = normal;
                Leo_Name.SetActive(true);
                Leo_Image.gameObject.SetActive(true);
                break;

            case "Lanz":
                Lanz_Image.color = normal;
                Lanz_Name.SetActive(true);
                Lanz_Image.gameObject.SetActive(true);
                break;

            case "YYY":
                YYY_Image.color = normal;
                YYY_Name.SetActive(true);
                YYY_Image.gameObject.SetActive(true);
                break;
        }
    }



    [Header("分支回答")]
    public AVGChoice avgChoice;

    private static string[] GetChoiceLabels(int firstQuestion, int language)
    {
        if (firstQuestion == 204)
        {
            return language switch
            {
                0 => new[] { "追われる理由", "服装と角について", "敵の正体", "話を切り上げる" },
                2 => new[] { "追問被追捕原因", "詢問衣著與犄角疑點", "盤問敵人勢力來歷", "就此收尾" },
                3 => new[] { "Why they chased you", "Clothes and horns", "Who are they?", "Wrap up" },
                4 => new[] { "쫓기는 이유", "옷과 뿔에 대해", "적의 정체", "대화 마치기" },
                _ => null
            };
        }

        return language switch
        {
            0 => new[] { "奪われたもの", "会社との関係", "特殊能力について", "もう十分" },
            2 => new[] { "追問被搶走的物品", "詢問與公司的關係", "詢問自身特殊能力", "就這樣吧" },
            3 => new[] { "What did they take?", "Why they want you", "Your special ability", "That's all" },
            4 => new[] { "빼앗긴 물건", "회사와의 관계", "특수 능력에 대해", "이 정도면 됐어" },
            _ => new[] { "追问被抢走的物品", "询问与公司的关系", "询问自身特殊能力", "就这样吧" }
        };
    }

    //快进按钮触发在这里
    public void ChangeStory()
    {


        switch (animation_number) 
        {

            case 203://开始
            case 204://A
            case 205://B
            case 206://C
                ShowChoices(204);
                break;

            case 401:
            case 402:
            case 403:
            case 404:
                ShowChoices(402);
                break;

            case 207://就此收尾
                GameFlowData.SinQuestionsCompleted = true;
                goto default;

            default:
                UIManager.instance.CloseAVG();
                gameObject.SetActive(false);
                break;
        }

      


        //AudioManager.instance.AudioPlay(AudioManager.instance.Attack_hit2);
    }



    private void ShowChoices(int firstQuestion)
    {
        StopAllCoroutines();
        CancelInvoke(nameof(Read));
        textLabel.text = "";
        int language = PlayerPrefs.GetInt("language", 0);
        avgChoice.OpenChoice(animation_number, firstQuestion, GetChoiceLabels(firstQuestion, language));
    }

    public void LoadChoice(int number)
    {
        animation_number = number;

        // 停止旧文本播放
        StopAllCoroutines();
        CancelInvoke(nameof(Read));

        // 重置打字状态
        index = 0;
        textFinished = true;
        cancelTyping = false;

        textLabel.text = "";

        // 重新读取对应TXT
        Read();
    }
}
