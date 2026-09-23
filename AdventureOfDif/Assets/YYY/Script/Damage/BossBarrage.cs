using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBarrage : MonoBehaviour
{
    [Header("预警")]
    public GameObject warningPrefab;

    [Header("爆炸")]
    public GameObject explosionPrefab;

    [Header("爆炸位置")]
    public Transform[] spawnPoints;
    List<Vector3> barragePositions = new List<Vector3>();

    [Header("预警出现间隔")]
    public float interval = 0.15f;

    [Header("全部预警完成后等待时间")]
    public float warningTime = 1f;

    private Coroutine barrageCoroutine;

    private void OnEnable()
    {
        barrageCoroutine = StartCoroutine(PlayBarrage());
    }

    IEnumerator PlayBarrage()
    {
        List<GameObject> warnings = new List<GameObject>();
        List<Vector3> barragePositions = new List<Vector3>();

        // ① 预警时锁定世界坐标
        foreach (Transform point in spawnPoints)
        {
            Vector3 pos = point.position;

            // ★记录这一刻的位置
            barragePositions.Add(pos);

            GameObject warning = Instantiate(
                warningPrefab,
                pos,
                Quaternion.identity
            );

            warnings.Add(warning);

            yield return new WaitForSeconds(interval);
        }

        // ② 等待爆炸
        yield return new WaitForSeconds(warningTime);

        // ③ 红点消失
        foreach (GameObject warning in warnings)
        {
            if (warning != null)
                Destroy(warning);
        }

        // ④ 在之前记录的位置爆炸
        foreach (Vector3 pos in barragePositions)
        {
            GameObject blast = Instantiate(
                explosionPrefab,
                pos,
                Quaternion.identity
            );

            Destroy(blast, 1f);
        }

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (barrageCoroutine != null)
        {
            StopCoroutine(barrageCoroutine);
            barrageCoroutine = null;
        }
    }
}