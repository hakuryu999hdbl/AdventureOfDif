using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public bool roomLeft, roomRight, roomUp, roomDown;
    //看清距离起点的距离
    public int stepToStart;
    public Text text;
    public int doorNumber;

    public GameObject ShowColor;

    public void UpdateRoom()
    {
        //计算距离初始点的网格距离
        stepToStart = (int)(Mathf.Abs(transform.position.x / 70) + Mathf.Abs(transform.position.y / 15));

        text.text = stepToStart.ToString();

        if (roomUp)
            doorNumber++;
        if (roomDown)
            doorNumber++;
        if (roomLeft)
            doorNumber++;
        if (roomRight)
            doorNumber++;
    }


    //因为看不清而增加
    public void SetStartRoom()
    {
        ShowColor.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);
    }
    public void SetEndRoom()
    {
        ShowColor.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
    }

}
