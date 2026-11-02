using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, IInteractable
{
    public Animator anim;
    public int Number;

    public void TriggerAction()
    {
        if (UIManager.instance.CurrentChooseMenu == -2)
            return;

      

        UIManager.instance.OpenAVG(Number, this);

        anim.Play("talk");
    }

    // 对话结束，返回待机
    public void StopTalk()
    {
        anim.Play("stand");
    }
}
