using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AreaEncounterController : MonoBehaviour
{
    public GameObject[] Go;//清理区域解锁下一个场景入口
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
            Invoke("ActivateArea", 0.5f);//有些场景直接出来会碰到来不及触发
            //ActivateArea();
        }
    }

    void ActivateArea()
    {
        areaActivated = true;

        // 1. 设置相机边界
        //RoomGenerator.SetNewBounds(cameraBoundsCollider);

        // 2. 封锁通路
        //if (blockade != null)
        //    blockade.SetActive(true);

        // 3. 生成敌人
        foreach (GameObject enemyPrefab in enemyPrefabs)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(1.5f, 0.5f);
            Vector3 spawnPosition = spawnPoint.position + new Vector3(offset.x, offset.y, 0);

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
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

                RoomGenerator.GoGo.SetActive(true);//提示清理完毕

                if (Go != null)
                {
                    foreach (GameObject g in Go)
                    {
                        if (g != null)
                            g.SetActive(true);
                    }
                }//离开场景入口展示

                Destroy(gameObject); // 删除这个触发器（可选）
                yield break;
            }
        }
    }




  
}