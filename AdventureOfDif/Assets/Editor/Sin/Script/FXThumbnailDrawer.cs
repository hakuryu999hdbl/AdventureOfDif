using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[InitializeOnLoad]
public static class FXThumbnailDrawer
{
    // 缓存 Prefab 名称 -> 图标
    static Dictionary<string, Texture2D> iconCache = new Dictionary<string, Texture2D>();

    // 图标存放路径（Editor 下）
    static string cacheFolder = "Assets/Editor/Sin/PbIcon_Sin";

    // 只对这个路径下的 Prefab 生效
    static string watchFolder = "Assets/Resources/VFX/Prefab";

    static FXThumbnailDrawer()
    {
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        
        // // 延迟刷新 Project 窗口，确保初次显示自定义图标
         EditorApplication.delayCall += () => EditorApplication.RepaintProjectWindow();
    }

    static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);

        // 只处理指定文件夹下的 Prefab
        if (!path.StartsWith(watchFolder) || !path.EndsWith(".prefab"))
            return;

        string prefabName = Path.GetFileNameWithoutExtension(path);

        // 延迟缓存：第一次访问这个 prefab 时加载图标
        if (!iconCache.ContainsKey(prefabName))
        {
            string iconPath = Path.Combine(cacheFolder, prefabName + ".png");
            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            iconCache[prefabName] = icon; // 如果没找到也缓存 null 避免重复查找
        }

        Texture2D cachedIcon = iconCache[prefabName];
        if (cachedIcon == null) return;

        // 判断 Project 窗口是否为缩略图模式（高度大于20基本就是缩略图视图）
        if (selectionRect.height > 20)
        {
            // 缩略图模式：预留边距，保证不超出
            float margin = 10f; // 上下左右边距
            Rect drawRect = new Rect(
                selectionRect.x + margin,
                selectionRect.y + margin * 0.1f,
                selectionRect.width - 2 * margin,
                selectionRect.height - 2 * margin
            );

            // 保持图片宽高比，居中显示
            float aspect = (float)cachedIcon.width / cachedIcon.height;
            float rectAspect = drawRect.width / drawRect.height;

            if (aspect > rectAspect)
            {
                // 图片更宽，按宽度缩放
                float height = drawRect.width / aspect;
                drawRect.y += (drawRect.height - height) / 2f;
                drawRect.height = height;
            }
            else
            {
                // 图片更高，按高度缩放
                float width = drawRect.height * aspect;
                drawRect.x += (drawRect.width - width) / 2f;
                drawRect.width = width;
            }

            GUI.DrawTexture(drawRect, cachedIcon, ScaleMode.ScaleToFit);
        }
        else
        {
            // 列表模式，在左边显示小图标
            Rect r = new Rect(selectionRect.x, selectionRect.y, selectionRect.height, selectionRect.height);
            GUI.DrawTexture(r, cachedIcon, ScaleMode.ScaleToFit);
        }
    }
}
    // static string cacheFolder = "Assets/Editor/PbIcon_Sin/FXIcons"; // 图标存放路径
    // static string watchFolder = "Assets/Resources/VFX/Prefab"; // 只对这个路径下的 Prefab 生效