using System.Collections.Generic;
using UnityEngine;

public class SpineGhostTrail : MonoBehaviour
{
    [Header("基础生成控制")]
    [Range(1, 6)] public int maxGhostCount = 4;
    public float distanceSpacing = 0.5f;          // 移动时的间距
    public float ghostLifeTime = 1.0f;            // 残影持续时间

    [Header("攻击状态控制")]
    public string attackLayerName = "Attack Layer";
    public float attackSpawnInterval = 0.1f;      // 攻击时产生残影的频率(秒)
    // 攻击状态名列表
    private readonly string[] attackStateNames = { "attack_1", "attack_2", "attack_3", "attack_4", "run_attack" };
    private HashSet<int> attackStateHashes = new HashSet<int>();

    [Header("颜色与透明度")]
    [GradientUsage(true)] public Gradient colorGradient;

    [Header("引用")]
    public Material ghostMaterial;
    private Animator animator;
    private int attackLayerIndex = -1;

    private MeshFilter mainMeshFilter;
    private MeshRenderer mainMeshRenderer;
    private MaterialPropertyBlock propBlock;

    // 状态记录
    private Vector3 lastSpawnPosition;
    private Vector3 lastFramePosition;
    private bool isFacingRight = true;
    private float attackTimer; // 专门用于攻击状态的计时器

    private class GhostInstance {
        public GameObject go;
        public MeshFilter mf;
        public MeshRenderer mr;
        public float spawnTime;
        public bool isActive;
        public void SetActive(bool active) { isActive = active; go.SetActive(active); }
    }

    private List<GhostInstance> ghostPool = new List<GhostInstance>();

    void Awake() {
        mainMeshFilter = GetComponent<MeshFilter>();
        mainMeshRenderer = GetComponent<MeshRenderer>();
        animator = GetComponent<Animator>();
        propBlock = new MaterialPropertyBlock();
        
        lastSpawnPosition = transform.position;
        lastFramePosition = transform.position;

        // 初始化攻击状态的 Hash，提高查询效率
        foreach (var name in attackStateNames) {
            attackStateHashes.Add(Animator.StringToHash(name));
        }

        // 获取攻击层索引
        attackLayerIndex = animator.GetLayerIndex(attackLayerName);

        for (int i = 0; i < maxGhostCount; i++) {
            ghostPool.Add(CreateNewGhostInstance());
        }
    }

    GhostInstance CreateNewGhostInstance() {
        GameObject go = new GameObject("SpineGhost_Instance");
        GhostInstance instance = new GhostInstance {
            go = go,
            mf = go.AddComponent<MeshFilter>(),
            mr = go.AddComponent<MeshRenderer>(),
            isActive = false
        };
        instance.mr.material = ghostMaterial;
        go.SetActive(false);
        return instance;
    }

    void Update() {
        // 1. 判定当前朝向 (仅在有位移时更新)
        float moveDeltaX = transform.position.x - lastFramePosition.x;
        if (Mathf.Abs(moveDeltaX) > 0.001f) {
            isFacingRight = moveDeltaX > 0;
        }
        lastFramePosition = transform.position;

        // 2. 检测当前是否处于攻击状态
        bool isAttacking = CheckIfAttacking();

        if (isAttacking) {
            // 攻击模式：基于时间生成
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackSpawnInterval) {
                SpawnGhost();
                attackTimer = 0;
                lastSpawnPosition = transform.position; // 同步位移基准，防止切回移动模式时瞬间多出一个
            }
        } else {
            // 普通模式：基于位移生成
            attackTimer = 0;
            float distFromLastSpawn = Vector3.Distance(transform.position, lastSpawnPosition);
            if (distFromLastSpawn >= distanceSpacing) {
                SpawnGhost();
                lastSpawnPosition = transform.position;
            }
        }

        // 3. 更新残影表现
        UpdateGhosts();
    }

    bool CheckIfAttacking() {
        if (attackLayerIndex == -1) return false;

        // 获取当前层的动画状态信息
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(attackLayerIndex);
        
        // 检查当前状态的 fullPathHash 或 shortNameHash 是否在我们的攻击列表里
        return attackStateHashes.Contains(stateInfo.shortNameHash);
    }

    void SpawnGhost() {
        if (mainMeshFilter.sharedMesh == null) return;

        GhostInstance ghost = GetBestGhostToReuse();

        if (ghost.mf.sharedMesh != null) Destroy(ghost.mf.sharedMesh);
        ghost.mf.mesh = Instantiate(mainMeshFilter.sharedMesh);

        ghost.go.transform.position = transform.position;
        Vector3 newScale = transform.localScale;
        // 关键：这里会沿用上一次 moveDeltaX 确定的 isFacingRight
        newScale.x = isFacingRight ? Mathf.Abs(newScale.x) : -Mathf.Abs(newScale.x);
        ghost.go.transform.localScale = newScale;
        ghost.go.transform.rotation = transform.rotation;

        ghost.mr.sortingLayerID = mainMeshRenderer.sortingLayerID;
        ghost.mr.sortingOrder = mainMeshRenderer.sortingOrder ;//这里暂时一致

        ghost.spawnTime = Time.time;
        ghost.SetActive(true);
    }

    void UpdateGhosts() {
        for (int i = 0; i < ghostPool.Count; i++) {
            var ghost = ghostPool[i];
            if (!ghost.isActive) continue;

            float age = Time.time - ghost.spawnTime;
            float normalizedAge = age / ghostLifeTime;

            if (normalizedAge >= 1.0f) {
                ghost.SetActive(false);
            } else {
                Color tintColor = colorGradient.Evaluate(normalizedAge);
                ghost.mr.GetPropertyBlock(propBlock);
                propBlock.SetColor("_Color", tintColor);
                propBlock.SetTexture("_MainTex", mainMeshRenderer.sharedMaterial.mainTexture);
                ghost.mr.SetPropertyBlock(propBlock);
            }
        }
    }

    GhostInstance GetBestGhostToReuse() {
        foreach (var g in ghostPool) if (!g.isActive) return g;
        GhostInstance oldest = ghostPool[0];
        foreach (var g in ghostPool) if (g.spawnTime < oldest.spawnTime) oldest = g;
        return oldest;
    }

    void OnDestroy() {
        foreach (var g in ghostPool) {
            if (g.mf && g.mf.sharedMesh) Destroy(g.mf.sharedMesh);
            if (g.go) Destroy(g.go);
        }
    }
}