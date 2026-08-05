using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    public GameObject transitionUI; // 预设体，Prefab
    public Material transitionMaterial; // 材质，控制_FillValue
    public GameObject img;

    public static SceneTransitionController Instance { get; private set; }

    private bool isTransitioning;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning(
                $"[SceneTransition] 删除重复实例：{gameObject.name}",
                gameObject
            );

            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartGame(string sceneName)
    {
        if (isTransitioning)
        {
            Debug.LogWarning(
                $"[SceneTransition] 正在切换场景，忽略重复请求：{sceneName}"
            );
            return;
        }

        StartCoroutine(DoSceneTransition(sceneName));
    }

    IEnumerator DoSceneTransition(string sceneName)
    {
        isTransitioning = true;

        Debug.Log(
            $"[SceneTransition] 开始加载：{sceneName}，" +
            $"nextAreaId='{GameFlowData.nextAreaId}'"
        );




        img.SetActive(true);

        transitionUI.SetActive(true);

        // _FillValue: -2 ➜ 2
        yield return StartCoroutine(AnimateFill(-2f, 2f, 1.0f));

        // 加载场景
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);


        while (!async.isDone)
        {
            yield return null;
        }

        // _FillValue: 2 ➜ -2
        yield return StartCoroutine(AnimateFill(2f, -2f, 1.0f));

        transitionUI.SetActive(false);

        img.SetActive(false);

        isTransitioning = false;
    }

    IEnumerator AnimateFill(float from, float to, float duration)
    {
        float timer = 0;


        while (timer < duration)
        {
            float value = Mathf.Lerp(from, to, timer / duration);
            transitionMaterial.SetFloat("_FillValue", value);


            timer += Time.deltaTime;
            yield return null;
        }

        transitionMaterial.SetFloat("_FillValue", to);
    }
}