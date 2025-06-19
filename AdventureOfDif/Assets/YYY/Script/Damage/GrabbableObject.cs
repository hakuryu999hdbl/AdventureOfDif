using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour
{
    public enum GrabbableType { Tanker, Inbox }

    public GrabbableType item = GrabbableType.Tanker;

    public SpriteRenderer spriteRenderer;
    public Sprite tanker, inbox;

    void Start() 
    {
        switch(Random.Range(0,2))
        {
            case 0:
                SetSkin(GrabbableType.Tanker);
                break;
            case 1:
                SetSkin(GrabbableType.Inbox);
                break;
        }
    
    }

    GrabbableObject.GrabbableType heldItemType;
    public void SetSkin(GrabbableObject.GrabbableType item) 
    {

        switch (item)
        {
            case GrabbableType.Tanker:
                spriteRenderer.sprite = tanker;
                break;
            case GrabbableType.Inbox:
                spriteRenderer.sprite = inbox;
                break;
        }

        heldItemType = item;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<Player>() != null)
            {
                if (collision.gameObject.GetComponent<Player>().isRunning || collision.gameObject.GetComponent<Player>().zHeight > 0.01f || collision.gameObject.GetComponent<Player>().isHoldingObject)//玩家跑步，在空中,已经有物品的时候，无法举起物品
                { return; }

                collision.gameObject.GetComponent<Player>().OnGrabCollision(heldItemType);//玩家举起油罐

                Destroy(gameObject);
            }
        }
    }
}
