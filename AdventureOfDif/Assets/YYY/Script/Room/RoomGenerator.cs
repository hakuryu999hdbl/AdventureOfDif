using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Cinemachine;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{
    public static RoomGenerator instance { get; private set; }
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

        //根据当前临时存档读取位置
        switch (GameFlowData.nextAreaId) 
        {
         
            //大路
            case "Area01_1":
            case "Area01_2":
            case "Area01_3":
            case "Area01_4":
                SetArea(0);
                break;
           
            //停车场
            case "Area02_1":
            case "Area02_2":
                SetArea(1);
                break;


            
            //仓库
            case "Area03_1":
                SetArea(2);
                break;

           
            //小巷
            case "Area04_1":
            case "Area04_2":
                SetArea(3);
                break;

           
            //健身房外部街道
            case "Area05_1":
            case "Area05_2":
            case "Area05_3":
                SetArea(4);
                break;

            //default:
            //健身房大厅
            case "Area06_1":
            case "Area06_2":
                SetArea(5);
                break;

           
            //健身房室内
            case "Area07_1":
                SetArea(6);
                break;

            default:
                //Debug.LogError(
                //    $"[RoomGenerator] 无法识别 nextAreaId：'{GameFlowData.nextAreaId}'，" +
                //    "请检查 AreaExit 的 ExitName 或 GameFlowData 是否被重置。"
                //);

                // 临时安全出生点，默认健身房大厅
                GameFlowData.nextAreaId = "Area06_1";
                SetArea(5);
                break;
        }

       


        Scan();




        player.character.LoadTempState();


        //Invoke("SetEnemy", 1f);
        //Invoke("SetEnemy", 1.5f);
        //Invoke("SetEnemy", 2f);
        //Invoke("SetEnemy", 2.5f);
        //Invoke("SetEnemy", 3f);
        //Invoke("SetEnemy", 3.5f);

    }


    public PlayerController player;

    /// <summary>
    /// 关卡
    /// </summary>
    #region
    [Header("关卡")]

    public List<GameObject> areaList; // 在Inspector中添加Area_1~3
    //private int currentAreaIndex = 0;



    public CinemachineConfiner confiner;//摄像机边界
    public GameObject Player;//开头把玩家送到地图入口

    public void SetArea(int index) 
    {




        GameObject NewArea = Instantiate(areaList[index], Vector3.zero, Quaternion.identity);


        // 找到新区域里的 CameraBounds（PolygonCollider2D）
        PolygonCollider2D newBounds = NewArea.transform.Find("CameraBounds").GetComponent<PolygonCollider2D>();
        SetNewBounds(newBounds);


        // 把玩家的位置设为这个出生点

        switch (GameFlowData.nextAreaId)
        {
            default:
            case "Area01_1":
            case "Area02_1":
            case "Area03_1":
            case "Area04_1":
            case "Area05_1":
            case "Area06_1":
            case "Area07_1":
                Transform PlayerPoint = NewArea.transform.Find("PointForPlayer_1");
                Player.transform.position = PlayerPoint.position;
                break;

            case "Area01_2":
            case "Area02_2":
            case "Area04_2":
            case "Area05_2":
            case "Area06_2":
                Transform PlayerPoint_2 = NewArea.transform.Find("PointForPlayer_2");
                Player.transform.position = PlayerPoint_2.position;
                break;


            case "Area05_3":
            case "Area01_3":
                Transform PlayerPoint_3 = NewArea.transform.Find("PointForPlayer_3");
                Player.transform.position = PlayerPoint_3.position;
                break;
            case "Area01_4":
                Transform PlayerPoint_4 = NewArea.transform.Find("PointForPlayer_4");
                Player.transform.position = PlayerPoint_4.position;
                break;
        }

       
    }


    public void SetNewBounds(PolygonCollider2D newBounds)
    {
        confiner.m_BoundingShape2D = newBounds;
        confiner.InvalidatePathCache(); // 强制刷新路径缓存，防止摄像机卡住
    }//放置相机边界


    public void LoadNextArea() 
    {
        //当玩家进入下一给场景的时候记录数值
        player.character.SaveTempState();




        SceneTransitionController transition = FindFirstObjectByType<SceneTransitionController>();

        if (transition != null)
        {
            transition.StartGame("YYY");
            //FindObjectOfType<SceneTransitionController>().StartGame("YYY");
        }
        else
        {
            //为了直接打开YYY场景也可以跳转
            SceneManager.LoadScene("YYY", LoadSceneMode.Single);
        }

    }

    public void LoadShop() 
    {
        SceneTransitionController transition = FindFirstObjectByType<SceneTransitionController>();
        transition.StartGame("Shop");
    }

    #endregion


    /// <summary>
    /// 游戏结束
    /// </summary>
    #region
    [Header("其他设置")]
    public bool gameOver = false;//玩家死亡游戏结束

    #endregion

    /// <summary>
    /// 设置寻路，离开场景进入地图标准与寻路
    /// </summary>
    #region
    [Header("设置寻路")]
    public AstarPath AstarPath;
    void Scan()
    {
        AstarPath.Scan();
    }



    #endregion




    /// <summary>
    /// 连击数字显示
    /// </summary>
    #region
    [Header("连击显示")]
    private int currentCombo = 0;
    private float comboTimer = 0f;
    public float comboResetTime = 3f; // 3秒内没打人就归零

    public GameObject comboTextUI;
    public Text comboText;

    private void Update()
    {
        if (currentCombo > 0)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer >= comboResetTime)
            {
                ResetCombo();
            }
        }
    }

    public void AddCombo()
    {
        currentCombo++;
        comboTimer = 0f;

        if (currentCombo >= 3)
        {
            comboTextUI.SetActive(true);
            comboText.text = "Combo x" + currentCombo;
        }
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        comboTextUI.SetActive(false);
        comboTimer = 0f;
    }


    #endregion

    /// <summary>
    /// 前进提示
    /// </summary>
    #region
    public GameObject GoGo;
    #endregion






}

