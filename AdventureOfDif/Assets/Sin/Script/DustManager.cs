using UnityEngine;

public class DustManager : MonoBehaviour
{
    public enum Mode { None, Walk, Run }

    public Animator CharAnimator;
    [Header("路径：Resources/VFX/Prefab/DustWalk & DustRun")]
    public string walkDustPath = "VFX/Prefab/VFX_Dust_Walk";
    public string runDustPath  = "VFX/Prefab/VFX_Dust_Run";

    [Header("间隔设置")]
    public float walkSpawnDistance = 0.9f;
    public float runSpawnDistance = 0.5f;

    private Mode currentMode = Mode.Walk;
    private Vector3 lastSpawnPos;

    private GameObject walkDustPrefab;
    private GameObject runDustPrefab;
    private Vector3 lastPosition;

    [Header("在角色前方多远生成")]
    public float GenerateOffsetWalk = 0.5f;
    public float GenerateOffsetRun = 0.8f;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Awake()
    {
        walkDustPrefab = Resources.Load<GameObject>(walkDustPath);
        runDustPrefab  = Resources.Load<GameObject>(runDustPath);

        if (!walkDustPrefab) Debug.LogError($"无法加载走路烟尘：Resources/{walkDustPath}");
        if (!runDustPrefab) Debug.LogError($"无法加载跑步烟尘：Resources/{runDustPath}");
    }

    public void SetDustMode(Mode mode)
    {
        currentMode = mode;
    }

    void Update()
    {
        if(CharAnimator!= null)
        {
            var stateInfo = CharAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("run")||stateInfo.IsName("grab_run"))
            {
                currentMode = Mode.Run;
                //Debug.Log("run");
            }
            else if (stateInfo.IsName("walk")||stateInfo.IsName("grab_walk"))
            {
                currentMode = Mode.Walk;
                //Debug.Log("walk");
            }
            else
            {
                currentMode = Mode.None;
            }
        }
        if (currentMode == Mode.None) return;

        float dist = Vector3.Distance(transform.position, lastSpawnPos);
        float spawnDist = (currentMode == Mode.Walk) ? walkSpawnDistance : runSpawnDistance;

        if (dist >= spawnDist)
        {
            SpawnDust();
            lastSpawnPos = transform.position;
        }
    }

    void SpawnDust()
    {
        GameObject prefabToSpawn = (currentMode == Mode.Walk) ? walkDustPrefab : runDustPrefab;
        if (!prefabToSpawn) return;

        // 获取角色速度
        Vector3 moveDir = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        // 计算反方向
        Vector3 sprayDir = -moveDir.normalized;

        // 计算 Z 旋转角（2D 模式下，atan2 返回的是弧度，转成角度）
        float angle = Mathf.Atan2(sprayDir.y, sprayDir.x) * Mathf.Rad2Deg;

        //烟雾生成的地点在角色前面一点,这样能跟上角色
        float offset = (currentMode == Mode.Walk) ? GenerateOffsetWalk : GenerateOffsetRun;
        Vector3 SpawnPos = transform.position - sprayDir * offset;
        angle += 180f;

        // 生成烟尘，并旋转到喷射方向
        Instantiate(prefabToSpawn, SpawnPos, Quaternion.Euler(0, 0, angle));
    }
}
