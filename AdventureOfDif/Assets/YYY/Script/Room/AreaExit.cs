using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaExit : MonoBehaviour
{
    private bool triggered = false;

    RoomGenerator RoomGenerator;//寻找RoomGenerator

    public String ExitName;

    void Start() 
    {
        //寻找RoomGenerator
        RoomGenerator = GameObject.FindGameObjectWithTag("RoomGenerator").GetComponent<RoomGenerator>();
    }

    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            triggered = true;


            GameFlowData.nextAreaId = ExitName;//记录下一个前往区域
            RoomGenerator.LoadNextArea();
        }
    }
}