using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMap : MonoBehaviour
{
    GameObject mapSprite;


   RoomGenerator _RoomGenerator;//寻找RoomGenerator,用于传送自己坐标告知RoomGenerator玩家到哪个房间了
   
   private void Start()
   {
       _RoomGenerator = GameObject.FindGameObjectWithTag("RoomGenerator").GetComponent<RoomGenerator>();//寻找RoomGenerator
   }

    private void OnEnable()
    {
        mapSprite = transform.parent.GetChild(0).gameObject;//获取子物体
        mapSprite.SetActive(false);


    }


    public void ShowMap()
    {
        mapSprite.SetActive(true);

    }//玩家进入地图柜子等显示
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ShowMap();



            // 告知RoomGenerator玩家进入的房间位置
            Invoke("PlayerInThisRoomToRoomGenerator", 0.1f);
        }


    }//玩家进入显示房间小地图
    void PlayerInThisRoomToRoomGenerator()
    {
        // 获取房间的位置
        Vector3 roomPosition = transform.position;
        _RoomGenerator.SetPlayerRoom(roomPosition);
        //Debug.Log("玩家所在房间位置"+roomPosition);

    }//稍微晚一点
}
