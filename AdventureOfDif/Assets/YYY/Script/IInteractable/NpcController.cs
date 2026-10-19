using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, IInteractable
{
    public Animator anim;

    public void TriggerAction()
    {
        anim.Play("talk");
    }
}
