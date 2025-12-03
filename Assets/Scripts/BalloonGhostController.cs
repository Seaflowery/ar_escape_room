using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonGhostController : MonoBehaviour
{
    public static bool globalGhostSpawningEnabled = true;

    // Track all instances so we can destroy/reset them from XRPlayerHealth.
    private static readonly List<BalloonGhostController> allGhosts = new List<BalloonGhostController>();

    public static void DestroyAllGhosts()
    {
        var copy = new List<BalloonGhostController>(allGhosts);
        foreach (var g in copy)
        {
            if (g != null)
            {
                GameObject.Destroy(g.gameObject);
            }
        }
        allGhosts.Clear();
    }

    /// <summary>
    /// Reset the internal spawn timers for all ghosts.
    /// Call this from XRPlayerHealth when restarting the game.
    /// </summary>
    public static void ResetGlobalSpawnClock()
    {
        foreach (var g in allGhosts)
        {
            if (g != null)
            {
                g.spawnTimer = g.spawnInterval;   // wait full interval before next spawn
                g.isMoving = false;
                g.isAttacking = false;
                g.isWandering = false;
                g.HideGhost();
            }
        }
        // 注意：不要在这里改 globalGhostSpawningEnabled，由外部（XRPlayerHealth）控制
    }

    // ---------- Instance fields ----------

    [Header("Spawn Settings")]
    [Tooltip("Time interval between appearances, in seconds.")]
    public float spawnInterval = 5f;

    [Tooltip("How far forward from the camera the ghost should appear.")]
    public float spawnDistance = 3f;

    [Tooltip("Vertical offset above the camera's forward direction.")]
    public float verticalOffset = 1.5f;

    [Tooltip("Horizontal offset left/right from the camera's forward direction (used alternately).")]
    public float horizontalOffset = 1f;

    [Header("Movement")]
    [Tooltip("Movement speed toward the player.")]
    public float moveSpeed = 1.5f;

    [Tooltip("Distance at which the ghost stops and begins attacking.")]
    public float attackRange = 0.8f;

    [Header("Attack")]
    [Tooltip("Time interval between attacks, in seconds.")]
    public float attackInterval = 1f;

    [Tooltip("Damage dealt per attack.")]
    public int damagePerHit = 1;

    [Tooltip("If true, the ghost keeps attacking repeatedly once in range.")]
    public bool keepAttacking = true;

    [Header("Optional")]
    [Tooltip("If assigned, this camera will be used instead of Camera.main.")]
    public Transform explicitCameraTransform;
    public Vector3 cameraOffset = new Vector3(0f, -2.0f, 0f);

    [Header("Camera Tracking")]
    [Tooltip("How often to re-acquire the player camera (seconds).")]
    public float cameraRefreshInterval = 1.0f;
    private float cameraRefreshTimer = 0f;

    [Header("Line of Sight / Wandering")]
    [Tooltip("Layers considered as obstacles between ghost and player.")]
    public LayerMask obstructionMask;
    [Tooltip("How long the ghost wanders when blocked, in seconds.")]
    public float wanderDuration = 2f;
    [Tooltip("Wander radius around current position.")]
    public float wanderRadius = 1.5f;
    [Tooltip("Wander movement speed.")]
    public float wanderSpeed = 1f;

    private Transform cameraTransform;
    private XRPlayerHealth playerHealth;

    // per-instance state
    private float spawnTimer;
    private bool spawnOnLeft = true;
    private bool isMoving = false;
    private bool isAttacking = false;

    // wandering state
    private bool isWandering = false;
    private float wanderTimer = 0f;
    private Vector3 wanderTarget;

    // cache renderers for quick hide/show
    private Renderer[] cachedRenderers;

    // ---------- Unity lifecycle ----------

    private void Awake()
    {
        allGhosts.Add(this);
        cachedRenderers = GetComponentsInChildren<Renderer>(true);
    }

    private void OnDestroy()
    {
        allGhosts.Remove(this);
    }

    private void Start()
    {
        TryUpdateCameraTransform();

        if (cameraTransform == null)
        {
            Debug.LogError("BalloonGhostController: No camera found. " +
                           "Assign 'explicitCameraTransform' or ensure XR camera has MainCamera tag.");
        }

        // Find player health
        playerHealth = FindObjectOfType<XRPlayerHealth>();
        if (playerHealth == null)
        {
            Debug.LogError("BalloonGhostController: No XRPlayerHealth found in scene.");
        }

        spawnTimer = spawnInterval;

        // Start hidden until first spawn
        HideGhost();
    }

    private void Update()
    {
        cameraRefreshTimer -= Time.deltaTime;
        if (cameraRefreshTimer <= 0f)
        {
            TryUpdateCameraTransform();
            cameraRefreshTimer = cameraRefreshInterval;
        }

        if (!globalGhostSpawningEnabled)
            return;

        if (cameraTransform == null || playerHealth == null || playerHealth.IsDead)
            return;

        // Count down to next spawn
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && !isMoving && !isAttacking && !isWandering)
        {
            SpawnInFrontOfPlayer();
            spawnTimer = spawnInterval;
        }

        // State machine: wandering 优先于 moving
        // if (isWandering)
        // {
        //     Wander();
        // }
        // else if (isMoving && !isAttacking)
        // {
            MoveTowardsPlayer();
        // }
    }

    // ---------- Camera helper ----------

    private void TryUpdateCameraTransform()
    {
        if (explicitCameraTransform != null)
        {
            cameraTransform = explicitCameraTransform;
            return;
        }

        if (Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    // ---------- Spawn / movement / attack ----------

    private void SpawnInFrontOfPlayer()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("Camera Transform not assigned.");
            return;
        }

        Vector3 camPos = cameraTransform.position + cameraOffset;

        // Forward & right directions from camera
        Vector3 forward = cameraTransform.forward;
        Vector3 right   = cameraTransform.right;

        // Alternate left/right spawn
        Vector3 sideOffset = spawnOnLeft ? -right : right;
        spawnOnLeft = !spawnOnLeft;

        // Final spawn position
        transform.position = camPos
                           + forward * spawnDistance
                           + sideOffset * horizontalOffset;

        // Look at player
        transform.LookAt(cameraTransform.position);

        ShowGhost();
        isMoving = true;
        isAttacking = false;
        isWandering = false;
    }

    private void MoveTowardsPlayer()
    {
        if (cameraTransform == null) return;

        Vector3 targetPos = cameraTransform.position + cameraOffset;
        Vector3 dir = targetPos - transform.position;
        float distance = dir.magnitude;

        // 到达攻击距离，开始攻击
        if (distance <= attackRange)
        {
            isMoving = false;
            StartAttack();
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position,
                            dir.normalized,
                            out hit,
                            distance,
                            obstructionMask,
                            QueryTriggerInteraction.Ignore))
        {
            StartWander();
            return;
        }

        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
    }

    private void StartWander()
    {
        if (isWandering) return;

        isWandering = true;
        isMoving = false;
        isAttacking = false;

        wanderTimer = wanderDuration;
        PickNewWanderTarget();
    }

    private void PickNewWanderTarget()
    {
        Vector3 randomOffset = Random.insideUnitSphere * wanderRadius;
        randomOffset.y = 0f; 
        wanderTarget = transform.position + randomOffset;
    }

    private void Wander()
    {
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            isWandering = false;
            isMoving = true;
            return;
        }

        float step = wanderSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, wanderTarget, step);

        if (Vector3.Distance(transform.position, wanderTarget) <= 0.1f)
        {
            PickNewWanderTarget();
        }
    }

    private void StartAttack()
    {
        if (isAttacking) return;
        isAttacking = true;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        Debug.Log("camera transform: " + cameraTransform.transform.position);
        while (!playerHealth.IsDead && keepAttacking && globalGhostSpawningEnabled)
        {
            float distance = Vector3.Distance(transform.position, cameraTransform.position + cameraOffset);
            if (distance <= attackRange + 0.1f)
            {
                playerHealth.TakeDamage(damagePerHit);
            }

            if (playerHealth.IsDead || !globalGhostSpawningEnabled)
            {
                break;
            }

            yield return new WaitForSeconds(attackInterval);
        }

        isAttacking = false;
        HideGhost();
    }

    // ---------- Visual helpers ----------

    private void HideGhost()
    {
        // Move it far away
        transform.position = new Vector3(0f, -999f, 0f);

        // reset states
        isMoving = false;
        isAttacking = false;
        isWandering = false;

        // Optionally disable renderers
        if (cachedRenderers != null)
        {
            foreach (var r in cachedRenderers)
            {
                if (r != null)
                    r.enabled = false;
            }
        }
    }

    private void ShowGhost()
    {
        if (cachedRenderers != null)
        {
            foreach (var r in cachedRenderers)
            {
                if (r != null)
                    r.enabled = true;
            }
        }
    }
}
