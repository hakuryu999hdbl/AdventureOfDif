using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAVG : MonoBehaviour
{
    [Header("AVG剧情编号")]
    public int Number;
    public int Number_2;// 提供给 AreaEncounterController 调用
    private bool triggered = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            triggered = true;

            UIManager.instance.OpenAVG(Number);
            UIManager.instance.SaveOpenAVG(this);
            //Destroy(gameObject);
        }
    }

    // 提供给 AreaEncounterController 调用
    public void TriggerAVG()
    {  
        UIManager.instance.OpenAVG(Number_2);
        //Destroy(gameObject);
    }

}
