using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, IInteractable
{
    public Animator anim;
    public int Number;
    [SerializeField] private int dialogueAfterSinQuestions;

    public void TriggerAction()
    {
        if (UIManager.instance.CurrentChooseMenu == -2)
            return;

      

        int dialogue = GameFlowData.SinQuestionsCompleted && dialogueAfterSinQuestions > 0
            ? dialogueAfterSinQuestions
            : Number;
        UIManager.instance.OpenAVG(dialogue, this);

        anim.Play("talk");
    }

    // 对话结束，返回待机
    public void StopTalk()
    {
        anim.Play("stand");
    }
}
