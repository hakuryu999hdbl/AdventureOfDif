using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class RoomGenerator : MonoBehaviour
{
  

    // Start is called before the first frame update
    void Start()
    {

        Invoke("PlayRegionBGM", 0.2f);//让主菜单的音乐先行











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
    /// 区域背景音乐
    /// </summary>
    #region
    [Header("区域BGM")]
    public BGM BGM;//用于红区等背景音乐
    public int CurrentRegionSituation;
    public void PlayRegionBGM()
    {
        switch (CurrentRegionSituation)
        {
            case 1:
                BGM.AudioPlayBackgroundMusic(-1);//播放红区等背景音乐
                break;
            case 0:
            case 2:
            case 3:
                BGM.AudioPlayChaseMusic(-1);//播放红区等背景音乐
                break;
        }

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

