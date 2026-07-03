using UnityEngine;

public class EnemyThrowObject : MonoBehaviour
{
    [Header("飞行设置")]
    public float flyTime = 0.7f;
    public float arcHeight = 2.5f;

    [Header("命中特效")]
    public GameObject explosionPrefab;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float timer;
    private bool isFlying;

    public void Launch(Vector3 start, Vector3 target)
    {
        startPos = start;
        targetPos = target;

        timer = 0f;
        isFlying = true;

        transform.position = startPos;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isFlying) return;

        timer += Time.deltaTime;

        float t = timer / flyTime;
        t = Mathf.Clamp01(t);

        Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

        pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

        transform.position = pos;

        if (t >= 1f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        isFlying = false;

        if (explosionPrefab != null)
        {
            GameObject effect = Instantiate(explosionPrefab, targetPos, Quaternion.identity);
            Destroy(effect, 1.2f);
        }

        gameObject.SetActive(false);
    }
}