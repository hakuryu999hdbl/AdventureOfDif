using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAVG : MonoBehaviour
{
    private bool triggered = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            triggered = true;

            UIManager.instance.OpenAVG();

           
        }
    }
}
