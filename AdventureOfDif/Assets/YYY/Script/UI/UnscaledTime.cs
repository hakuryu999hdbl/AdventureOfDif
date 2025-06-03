using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledTime : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;//不受TimeScale影响

    }
}
