using UnityEngine;

public class Gate : MonoBehaviour, IInteractable
{
    public Animator anim;

    public void TriggerAction()
    {
        if (anim == null) return;

        anim.SetTrigger("Open");
    }
}