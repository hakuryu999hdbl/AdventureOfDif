using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    [Header("本房间的墙壁")]
    public SpriteRenderer wallRenderer;

    [Header("对方房间的BlackCover")]
    public SpriteRenderer otherRoomCover;

    [Header("透明度设置")]
    public float wallOpaque = 1f;         // 墙壁不透明
    public float wallHalf = 0.5f;         // 墙壁半透明
    public float coverHalf = 0.5f;        // BlackCover 半透明
    public float coverTransparent = 0f;   // BlackCover 全透明

    private void SetAlpha(SpriteRenderer sr, float alpha)
    {
        if (sr == null) return;
        Color c = sr.color;
        c.a = alpha;
        sr.color = c;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            if (collision.gameObject.GetComponent<Player>().IsGrounded()) //跳在空中不触发
            {
                // 玩家进入本房间
                SetAlpha(wallRenderer, wallOpaque);         // 本房间墙壁不透明
                //wallRenderer.gameObject.SetActive(true);
                SetAlpha(otherRoomCover, coverHalf);        // 对方 BlackCover 半透明
            }

           
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 玩家离开本房间
            SetAlpha(wallRenderer, wallHalf);           // 本房间墙壁半透明
            //wallRenderer.gameObject.SetActive(false);
            SetAlpha(otherRoomCover, coverTransparent);// 对方 BlackCover 透明
        }
    }
}
