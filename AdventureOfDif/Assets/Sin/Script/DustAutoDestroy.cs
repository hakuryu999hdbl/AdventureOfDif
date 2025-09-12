using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustAutoDestroy : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        var main = GetComponent<ParticleSystem>().main;
        main.stopAction = ParticleSystemStopAction.Destroy;
    }

    // void Update()
    // {
    //     // 当粒子系统播放完毕并且没有存活粒子时销毁
    //     if (ps && !ps.IsAlive(true))
    //     {
    //         Destroy(gameObject);
    //     }
    // }
}
