using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteOrderTrigger : MonoBehaviour
{
    [Header("需要临时提高层级的图片")]
    public List<SpriteRenderer> sprites = new List<SpriteRenderer>();

    [Header("进入时设置成的 sortingOrder 值")]
    public int targetOrder = 1;

    // 缓存原始层级
    private readonly Dictionary<SpriteRenderer, int> originalOrders = new Dictionary<SpriteRenderer, int>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        foreach (var sr in sprites)
        {
            if (sr == null) continue;

            // 首次记录原始层级
            if (!originalOrders.ContainsKey(sr))
                originalOrders[sr] = sr.sortingOrder;

            sr.sortingOrder = targetOrder;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        RestoreAll();
    }

    // 物体被禁用/销毁时也尝试恢复，避免玩家停留导致的异常状态
    private void OnDisable()
    {
        RestoreAll();
    }

    private void RestoreAll()
    {
        foreach (var sr in sprites)
        {
            if (sr == null) continue;

            if (originalOrders.TryGetValue(sr, out int order))
                sr.sortingOrder = order;
        }
    }
}
