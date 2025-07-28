using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Cinemachine;

public class RoomGenerator : MonoBehaviour
{
  

    // Start is called before the first frame update
    void Start()
    {





        SetArea();


        Scan();







        //Invoke("SetEnemy", 1f);
        //Invoke("SetEnemy", 1.5f);
        //Invoke("SetEnemy", 2f);
        //Invoke("SetEnemy", 2.5f);
        //Invoke("SetEnemy", 3f);
        //Invoke("SetEnemy", 3.5f);
        //
        //Invoke("SetFriend", 6f);
    }


    /// <summary>
    /// 关卡
    /// </summary>
    #region

    public GameObject Area_1, Area_2, Area_3;
    public CinemachineConfiner confiner;//摄像机边界
    public GameObject Player;//开头把玩家送到地图入口

    public void SetArea() 
    {
        GameObject NewArea =Instantiate(Area_3, transform.position, Quaternion.identity);

        // 找到新区域里的 CameraBounds（PolygonCollider2D）
        PolygonCollider2D newBounds = NewArea.transform.Find("CameraBounds").GetComponent<PolygonCollider2D>();
        SetNewBounds(newBounds);


        // 把玩家的位置设为这个出生点
       Transform PlayerPoint = NewArea.transform.Find("PointForPlayer_1");
       Player.transform.position = PlayerPoint.position;
    }


    public void SetNewBounds(PolygonCollider2D newBounds)
    {
        confiner.m_BoundingShape2D = newBounds;
        confiner.InvalidatePathCache(); // 强制刷新路径缓存，防止摄像机卡住
    }

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
    /// 设置敌人
    /// </summary>
    #region
    [Header("设置敌人")]
    public Transform player;
    public GameObject Enemy;
    public List<GameObject> enemyList = new List<GameObject>();

    public float spawnOffsetX = 2f; // 超出屏幕多少单位生成
    public float spawnY = 0f;       // 敌人固定高度
    public bool spawnFromRight = true; // 控制是否右边刷出


    public void SetEnemy()
    {
        // 随机决定从左侧还是右侧刷出
        bool spawnFromRight = Random.value > 0.5f;

        // 获取摄像机边缘位置（Viewport：x=0是左边，x=1是右边，y=0.5是屏幕中间高度）
        Vector3 screenEdge = Camera.main.ViewportToWorldPoint(
            new Vector3(spawnFromRight ? 1.1f : -0.1f, 0.5f, Camera.main.nearClipPlane + 5f));

        // 关键！把 Y 替换为玩家的 Y（或你希望的高度）
        float y = player.position.y;

        // 生成位置
        Vector3 spawnPos = new Vector3(screenEdge.x, y, 0);

        // 生成敌人
        GameObject NewEnemy = Instantiate(Enemy, spawnPos, Quaternion.identity);
        enemyList.Add(NewEnemy);

    }
    public void SetFriend()
    {



        GameObject NewEnemy = Instantiate(Enemy, transform.position, Quaternion.identity);
        enemyList.Add(NewEnemy);


        Enemy enemy = NewEnemy.transform.Find("Enemy").GetComponent<Enemy>();
        enemy.ConvertToFriend();


    }

    #endregion


}

