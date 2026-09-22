using System.Collections;
using UnityEngine;

public class BossBarrage : MonoBehaviour
{
    public GameObject explosionPrefab;

    [Header("爆炸位置")]
    public Transform[] spawnPoints;

    [Header("每个爆炸之间的间隔")]
    public float interval = 0.15f;

    private Coroutine barrageCoroutine;

    private void OnEnable()
    {
        barrageCoroutine = StartCoroutine(PlayBarrage());
    }

    IEnumerator PlayBarrage()
    {
        foreach (Transform point in spawnPoints)
        {
            GameObject blast=
            Instantiate(
                explosionPrefab,
                point.position,
                Quaternion.identity
            );

            Destroy(blast, 1f);

            yield return new WaitForSeconds(interval);
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