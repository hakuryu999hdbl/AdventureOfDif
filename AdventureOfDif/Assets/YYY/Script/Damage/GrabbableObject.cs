using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public enum GrabbableType { Tanker, Inbox }

    GrabbableType item = GrabbableType.Tanker;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Player>() != null)
            {
                if (collision.gameObject.GetComponent<Player>().isRunning || collision.gameObject.GetComponent<Player>().zHeight > 0.01f || collision.gameObject.GetComponent<Player>().isHoldingObject)//玩家跑步，在空中,已经有物品的时候，无法举起物品
                { return; }

                collision.gameObject.GetComponent<Player>().OnGrabCollision(item);//玩家举起油罐

                Destroy(gameObject);
            }
        }
    }
}
