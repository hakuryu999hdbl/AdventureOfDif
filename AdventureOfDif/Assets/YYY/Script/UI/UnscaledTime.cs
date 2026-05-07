using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledTime : MonoBehaviour
{
    Animator animator;

    void Start()
    {

        animator = GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;//不受TimeScale影响

    }
}
