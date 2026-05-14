using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState
{
    //在这里函数只需要声明抽象的函数类，不需要方法
    public abstract void EnterState(EnemyController enemy);//开始执行什么

    public abstract void OnUpdate(EnemyController enemy);//每帧执行什么



}