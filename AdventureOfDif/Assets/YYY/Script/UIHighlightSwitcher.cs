using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHighlightSwitcher : MonoBehaviour
{
    [Header("被激活时显示的目标物体")]
    public GameObject targetToShow;

    [Header("需要隐藏的其他物体列表")]
    public List<GameObject> othersToHide;

    // 被选中或触发时调用这个函数
    public void ActivateThis()
    {
        // 显示自己的
        if (targetToShow != null)
            targetToShow.SetActive(true);

        // 隐藏其他的
        foreach (var obj in othersToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}
