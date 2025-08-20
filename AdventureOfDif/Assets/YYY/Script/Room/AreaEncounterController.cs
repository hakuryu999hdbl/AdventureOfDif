using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AreaEncounterController : MonoBehaviour
{

    public RoomGenerator RoomGenerator;//寻找RoomGenerator

    [Header("敌人生成")]
    public GameObject[] enemyPrefabs; // 敌人预制体数组
    public Transform[] spawnPoints;   // 生成点数组
    private List<GameObject> aliveEnemies = new List<GameObject>();

    [Header("区域控制")]
    public GameObject blockade; // 用于封锁视野/通路的墙
    public PolygonCollider2D cameraBoundsCollider; // 此区域的边界，用于替换 confiner 的 bounds
    public PolygonCollider2D cameraBoundsCollider_All;

    [Header("摄像机控制")]
    //public CinemachineConfiner confiner;
    private bool areaActivated = false;//是否被触发一遍


    private void Start()
    {
        //寻找RoomGenerator
        RoomGenerator = GameObject.FindGameObjectWithTag("RoomGenerator").GetComponent<RoomGenerator>();

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!areaActivated && collision.CompareTag("Player"))
        {
            ActivateArea();
        }
    }

    void ActivateArea()
    {
        areaActivated = true;

        // 1. 设置相机边界
        RoomGenerator.SetNewBounds(cameraBoundsCollider);
        //confiner.m_BoundingShape2D = cameraBoundsCollider;
        //confiner.InvalidatePathCache();

        // 2. 封锁通路
        if (blockade != null)
            blockade.SetActive(true);

        // 3. 生成敌人
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            aliveEnemies.Add(enemy);
        }

        // 4. 监听敌人是否全部死亡
        StartCoroutine(CheckEnemiesDead());
    }

    System.Collections.IEnumerator CheckEnemiesDead()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            aliveEnemies.RemoveAll(e => e == null);

            if (aliveEnemies.Count == 0)
            {
                // 所有敌人已死亡，解除限制
                if (blockade != null)
                    blockade.SetActive(false);

                RoomGenerator.SetNewBounds(cameraBoundsCollider_All);

                Destroy(gameObject); // 删除这个触发器（可选）
                yield break;
            }
        }
    }




  
}