using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public GameObject Player;//全场景寻找玩家

    void FixedUpdate()
    {
        transform.position = Player.transform.position;


    }
}
