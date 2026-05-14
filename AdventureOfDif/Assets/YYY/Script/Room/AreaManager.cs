using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    public static AreaManager Instance;

    [Header("本区域巡逻点")]
    public List<Transform> patrolPoints = new List<Transform>();

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetRandomPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Count == 0)
            return null;

        return patrolPoints[Random.Range(0, patrolPoints.Count)];
    }

    public List<Transform> GetPatrolPoints()
    {
        return patrolPoints;
    }
}
