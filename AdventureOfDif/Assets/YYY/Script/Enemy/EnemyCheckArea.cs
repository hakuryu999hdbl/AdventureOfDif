using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckArea : MonoBehaviour
{
    [Header("传递消息")]
    public EnemyController enemy;



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!enemy.attackList.Contains(other.transform))
            {
                enemy.attackList.Add(other.transform);
            }



        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (enemy.attackList.Contains(other.transform))
            {
                enemy.attackList.Remove(other.transform);
            }
        }
    }
}
