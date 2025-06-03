using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class RoomGenerator : MonoBehaviour
{
    //枚举
    public enum Direction { up, down, left, right };//up0,down1,left3,right4;
    public Direction direction;

    [Header("房间信息")]
    public GameObject roomPrefab;
    public int roomNumber;
    public Color startColor, endColor;
    private GameObject endRoom;//最远的房间

    [Header("位置控制")]
    public Transform generatorPoint;
    public float xOffset;
    public float yOffset;
    public LayerMask roomLayer;//需要判断是否房间重叠，检测room层
    public int maxStep;

    public List<Room> rooms = new List<Room>();

    //筛选最适合做Boss战房间
    List<GameObject> farRooms = new List<GameObject>();
    List<GameObject> lessFarRooms = new List<GameObject>();
    List<GameObject> oneWayRooms = new List<GameObject>();


    //房间预设体
    GameObject L, R, U, B, LU, LR, LB, UR, UB, RB, LUR, LUB, URB, LRB, LURB;
    public WallType wallType;


    //当前区域内敌情
    public int CurrentRegionSituation;//1绿区 0白区 2黄区 3红区
    //当前区域内执行任务
    public int CurrentRegionMission;//0无任务 1红区歼灭 2红区袭击 3白区清剿 4黄区歼灭 5黄区袭击




    // Start is called before the first frame update
    void Start()
    {

        Invoke("PlayRegionBGM", 0.2f);//让主菜单的音乐先行





        //随机房间大小
        roomNumber = Random.Range(4, 8);

        ChoosePlace();

        //只有一个房间的时候
        if (roomNumber == 1) { Instantiate(L, new Vector3(0, 0, 0), Quaternion.identity); }

        for (int i = 0; i < roomNumber; i++)
        {
            rooms.Add(Instantiate(roomPrefab, generatorPoint.position, Quaternion.identity).GetComponent<Room>());//将生成的房间添加到列表
            ChangePointPos();//每生成后转移位置
        }

        rooms[0].GetComponent<Room>().SetStartRoom();
        //rooms[roomNumber - 1].GetComponent<SpriteRenderer>().color = endColor;

        endRoom = rooms[0].gameObject;
        foreach (var room in rooms)//检测每个房间
        {
            if (room.transform.position.sqrMagnitude > endRoom.transform.position.sqrMagnitude)//比较二者距离
            {
                endRoom = room.gameObject;
            }

            SetupRoom(room, room.transform.position);
        }
        FindEndRoom();
        endRoom.GetComponent<Room>().SetEndRoom();








        // 将玩家初始所在的房间设置为房间列表中的第一个房间
        playerRoom = rooms[0].GetComponent<Room>();
        // 游戏一开始RoomGenerator直接把当前所有房间的位置告诉Move_Target
        foreach (var room in rooms)
        {
            roomPositions.Add(room.transform.position);
        }








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



    void DelayRestart()
    {
        PlayerPrefs.SetInt("Situation", 0); //等下一次刷新就是重新开始了
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
    ///  生成房间
    /// </summary>
    #region
    public void ChoosePlace()
    {
        L = wallType.singleLeft;
        R = wallType.singleRight;
        U = wallType.singleUp;
        B = wallType.singleBottom;

        LU = wallType.doubleLU;
        LR = wallType.doubleLR;
        LB = wallType.doubleLB;

        UR = wallType.doubleUR;
        UB = wallType.doubleUB;
        RB = wallType.doubleRB;

        LUR = wallType.tripleLUR;
        LUB = wallType.tripleLUB;
        URB = wallType.tripleURB;
        LRB = wallType.tripleLRB;

        LURB = wallType.fourDoors;
    }//选择关卡

    public void ChangePointPos()
    {
        direction = (Direction)Random.Range(0, 4);

        do
            switch (direction)
            {
                case Direction.up://当方向是向上的时候
                    generatorPoint.position += new Vector3(0, yOffset, 0);
                    break;
                case Direction.down://当方向是向下的时候
                    generatorPoint.position += new Vector3(0, -yOffset, 0);
                    break;
                case Direction.left://当方向是向左的时候
                    generatorPoint.position += new Vector3(-xOffset, 0, 0);
                    break;
                case Direction.right://当方向是向右的时候
                    generatorPoint.position += new Vector3(xOffset, 0, 0);
                    break;
            } while (Physics2D.OverlapCircle(generatorPoint.position, 0.2f, roomLayer));
    }//更改生成点（随机方向移动位置

    public void SetupRoom(Room newRoom, Vector3 roomPosition)
    {
        newRoom.roomUp = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), 0.2f, roomLayer);//当前房间的位置加上向上位移
        newRoom.roomDown = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), 0.2f, roomLayer);//当前房间的位置加上向下位移
        newRoom.roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), 0.2f, roomLayer);//当前房间的位置加上向左位移
        newRoom.roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), 0.2f, roomLayer);//当前房间的位置加上向右位移

        newRoom.UpdateRoom();

        switch (newRoom.doorNumber)
        {
            case 1:
                if (newRoom.roomUp)
                    Instantiate(U, roomPosition, Quaternion.identity);
                if (newRoom.roomDown)
                    Instantiate(B, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft)
                    Instantiate(L, roomPosition, Quaternion.identity);
                if (newRoom.roomRight)
                    Instantiate(R, roomPosition, Quaternion.identity);
                break;
            case 2:
                if (newRoom.roomLeft && newRoom.roomUp)
                    Instantiate(LU, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft && newRoom.roomRight)
                    Instantiate(LR, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft && newRoom.roomDown)
                    Instantiate(LB, roomPosition, Quaternion.identity);
                if (newRoom.roomUp && newRoom.roomRight)
                    Instantiate(UR, roomPosition, Quaternion.identity);
                if (newRoom.roomUp && newRoom.roomDown)
                    Instantiate(UB, roomPosition, Quaternion.identity);
                if (newRoom.roomRight && newRoom.roomDown)
                    Instantiate(RB, roomPosition, Quaternion.identity);
                break;
            case 3:
                if (newRoom.roomLeft && newRoom.roomUp && newRoom.roomRight)
                    Instantiate(LUR, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft && newRoom.roomRight && newRoom.roomDown)
                    Instantiate(LRB, roomPosition, Quaternion.identity);
                if (newRoom.roomUp && newRoom.roomRight && newRoom.roomDown)
                    Instantiate(URB, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft && newRoom.roomUp && newRoom.roomDown)
                    Instantiate(LUB, roomPosition, Quaternion.identity);
                break;
            case 4:
                if (newRoom.roomLeft && newRoom.roomUp && newRoom.roomRight && newRoom.roomDown)
                    Instantiate(LURB, roomPosition, Quaternion.identity);
                break;
        }
    }//检测上下左右有没有房间

    public void FindEndRoom()
    {
        //最大数值 最远距离数字
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].stepToStart > maxStep)
                maxStep = rooms[i].stepToStart;
        }
        //获得最远房间和第二远
        foreach (var room in rooms)
        {
            if (room.stepToStart == maxStep)
                farRooms.Add(room.gameObject);
            if (room.stepToStart == maxStep - 1)
                lessFarRooms.Add(room.gameObject);
        }
        for (int i = 0; i < farRooms.Count; i++)
        {
            if (farRooms[i].GetComponent<Room>().doorNumber == 1)
                oneWayRooms.Add(farRooms[i]);//最远房间里的单侧门加入
        }

        for (int i = 0; i < lessFarRooms.Count; i++)
        {
            if (lessFarRooms[i].GetComponent<Room>().doorNumber == 1)
                oneWayRooms.Add(lessFarRooms[i]);//第二远远房间里的单侧门加入
        }
        if (oneWayRooms.Count != 0)
        {
            endRoom = oneWayRooms[Random.Range(0, oneWayRooms.Count)];
        }
        else
        {
            endRoom = farRooms[Random.Range(0, farRooms.Count)];
        }
    }//检测最远房间
    #endregion

    /// <summary>
    /// 房间实体
    /// </summary>
    #region
    [System.Serializable]//即使没有挂MonoBehaviour可以被系统识别
    public class WallType
    {
        public GameObject
            singleLeft, singleRight, singleUp, singleBottom,
            doubleLU, doubleLR, doubleLB, doubleUR, doubleUB, doubleRB,
            tripleLUR, tripleLUB, tripleURB, tripleLRB,
            fourDoors;

    }
    #endregion





    /// <summary>
    ///  任何物体的随机端口,传送到随机房间/距离玩家最远的房间
    /// </summary>
    #region
    [Header("设置随机端口终点")]
    public GameObject _Player;//玩家
    // 定义偏移量范围
    public float offsetRange = 2.0f;


    public void ChangeTargetPlace(GameObject MoveTarget, int WhichRoom)//0 玩家当前房间  1距离玩家最远房间  2除了玩家之外的随机房间  3随机找一个房间
    {



        //敌人死亡后再次选择最近的
        //Invoke("CheckNearestEnemy", 0.1f);//要让敌人重刷之后，过一会再触发

        // 生成随机偏移量
        float offsetX = Random.Range(-offsetRange, offsetRange);
        float offsetY = Random.Range(-offsetRange, offsetRange);



        switch (WhichRoom) 
        {
            case -1:

                //拉到玩家当前位置
                MoveTarget.transform.position = _Player.transform.position + new Vector3(offsetX, offsetY, 0f);
                break;
            case 0:

                //拉到玩家当前房间
                MoveTarget.transform.position = playerRoom.transform.position + new Vector3(offsetX, offsetY, 0f);
                break;
            case 1:

                // 找到距离玩家最远的房间
                Vector3 farthestRoomPosition = FindFarthestRoomFromPlayer();

                MoveTarget.transform.position = farthestRoomPosition += new Vector3(offsetX, offsetY, 0f);
                break;
            case 2:

                // 随机找一个非玩家所在的房间
                MoveTarget.transform.position = FindRandomRoomExceptPlayerRoom() + new Vector3(offsetX, offsetY, 0f);
                break;
            case 3:

                //随机找一个房间
                int randomIndex = Random.Range(0, roomPositions.Count);
                MoveTarget.transform.position = roomPositions[randomIndex] += new Vector3(offsetX, offsetY, 0f);

                break;

        }




    }

    //将玩家拉到房间中央
    public void SetPlayerToRoomCenter()
    {
        _Player.transform.position = playerRoom.transform.position;
    }



    Vector3 FindFarthestRoomFromPlayer()
    {

        if (roomPositions == null || roomPositions.Count == 0)
        {
            Debug.LogError("房间列表未初始化或为空，无法找到随机房间！");
            return Vector3.zero; // 返回一个默认值，防止报错
        }//如果没有一个房间的话就没有办法确认玩家不在的房间


        Vector3 playerPosition = _Player.transform.position;
        Vector3 farthestRoomPosition = Vector3.zero;
        float maxDistance = float.MinValue;

        // 遍历所有房间，找到距离玩家最远的房间
        foreach (var room in rooms)
        {
            float distanceToPlayer = Vector3.Distance(room.transform.position, playerPosition);

            if (distanceToPlayer > maxDistance)
            {
                maxDistance = distanceToPlayer;
                farthestRoomPosition = room.transform.position;
            }
        }

        return farthestRoomPosition;
    }//获取距离玩家最远的房间




    //知晓所有房间的位置列表
    public List<Vector3> roomPositions = new List<Vector3>();
    Vector3 FindRandomRoomExceptPlayerRoom()
    {
        List<Vector3> availableRooms = new List<Vector3>(roomPositions);

        // 移除玩家所在的房间
        availableRooms.Remove(playerRoom.transform.position);

        // 在剩下的房间中随机选择
        int randomIndex = Random.Range(0, availableRooms.Count);
        return availableRooms[randomIndex];
    }   // 随机找一个不是玩家所在的房间




    //各个房间的WallMap传送自己坐标给RoomGenerator告诉玩家所处房间
    public Room playerRoom; // 玩家当前所在的房间            
    public void SetPlayerRoom(Vector3 roomPosition)
    {
        foreach (var room in rooms)
        {
            if (room.transform.position == roomPosition)
            {
                playerRoom = room;
                break;
            }
        }
    }  // 设置玩家所在的房间 
    #endregion




    /// <summary>
    /// 设置敌人
    /// </summary>
    #region
    [Header("设置敌人")]
    public GameObject Enemy;
    //敌人列表
    public List<GameObject> enemyList = new List<GameObject>();

    public void SetEnemy()
    {



        GameObject NewEnemy = Instantiate(Enemy, transform.position, Quaternion.identity);
        enemyList.Add(NewEnemy);

        ChangeTargetPlace(NewEnemy,0);
    }
    public void SetFriend()
    {



        GameObject NewEnemy = Instantiate(Enemy, transform.position, Quaternion.identity);
        enemyList.Add(NewEnemy);


        Enemy enemy = NewEnemy.transform.Find("Enemy").GetComponent<Enemy>();
        enemy.ConvertToFriend();

        ChangeTargetPlace(NewEnemy, -1);
    }

    #endregion


}

