using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private TMP_Text promptTxt;
    private string promptText = "Run away and Hide from kumki !";

    public NavMeshAgent agent;
    public Transform player;
    public Animator animatorKumki;
    public float chaseSpeed = 5.75f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;

    [HideInInspector] public int patrolIndex = 0;
    [HideInInspector] public int patrolDirection = 1;

    [Header("Vision")]
    public float viewRadius = 20f;
    public float viewAngle = 120f;
    public LayerMask obstacleLayer;

    [Header("Attack")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    [HideInInspector] public float lastAttackTime;

    [Header("Search")]
    public int searchPoints = 6;
    public float searchRadiusStep = 2f;

    [HideInInspector] public Vector3 lastKnownPosition;
    [HideInInspector] public Vector3 heardSoundPosition;

    EnemyState currentState;
    AudioSource audiosrc;

    public static EnemyAI instance { get; private set; }

    // ---------------- OPTIMIZATION TIMERS ----------------
    float visionTimer;
    float visionInterval = 0.2f;

    float animTimer;
    float animInterval = 0.1f;

    float slopeTimer;
    float slopeInterval = 0.1f;

    private Vector3 smoothedNormal = Vector3.up;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        animatorKumki = GetComponent<Animator>();
        audiosrc = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = true;
        agent.updateUpAxis = false;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        SwitchState(new EnemyPatrolState(this));
    }

    void Update()
    {
        currentState?.Update();
        UpdateVision();
        UpdateAnimatorLogic();
        HandleAudioAndUI();
    }

    void LateUpdate()
    {
        HandleRotationAndSlope();
    }

    // ======================================================
    // OPTIMIZED VISION (throttled raycast)
    // ======================================================
    public bool CanSeePlayer()
    {
        Vector3 origin = transform.position + Vector3.up * 3f;
        Vector3 target = player.position + Vector3.up * 2f;

        Vector3 dir = target - origin;
        float dist = dir.magnitude;

        if (dist > viewRadius) return false;

        dir /= dist;

        if (Vector3.Angle(transform.forward, dir) > viewAngle * 0.5f)
            return false;

        if (Physics.Raycast(origin, dir, dist, obstacleLayer))
            return false;

        lastKnownPosition = player.position;
        return true;
    }

    void UpdateVision()
    {
        visionTimer += Time.deltaTime;

        if (visionTimer >= visionInterval)
        {
            visionTimer = 0f;

            if (player != null)
            {
                bool seen = CanSeePlayer();

                if (seen)
                {
                    if (promptTxt != null)
                        promptTxt.text = promptText;

                    if (SoundManager.Instance.IsMusicPlaying(Music.Anxious))
                        SoundManager.Instance.StopMusic();

                    if (!SoundManager.Instance.IsMusicPlaying(Music.chase))
                        SoundManager.Instance.PlayMusic(Music.chase, 0.7f);
                }
            }
        }
    }

    // ======================================================
    // ANIMATOR OPTIMIZED (throttled)
    // ======================================================
    void UpdateAnimator()
    {
        float speed01 = Mathf.Clamp01(agent.desiredVelocity.magnitude / chaseSpeed);

        animatorKumki.SetFloat("speed", speed01, 0.15f, Time.deltaTime);

        if (speed01 > 0.1f)
        {
            if (!audiosrc.isPlaying)
                audiosrc.Play();
        }
        else
        {
            if (audiosrc.isPlaying)
                audiosrc.Pause();
        }
    }

    void UpdateAnimatorLogic()
    {
        animTimer += Time.deltaTime;

        if (animTimer >= animInterval)
        {
            animTimer = 0f;
            UpdateAnimator();
        }
    }

    // ======================================================
    // ROTATION + SLOPE OPTIMIZED
    // ======================================================
    void HandleRotationAndSlope()
    {
        if (agent.velocity.sqrMagnitude < 0.05f)
            return;

        // rotation toward movement
        Quaternion rot = Quaternion.LookRotation(agent.velocity.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);

        // slope handling throttled
        slopeTimer += Time.deltaTime;

        if (slopeTimer >= slopeInterval)
        {
            slopeTimer = 0f;
            AlignRotationToSlope();
        }
    }

    // ORIGINAL FUNCTION (kept name + logic optimized)
    private void AlignRotationToSlope()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 2.5f))
        {
            smoothedNormal = Vector3.Slerp(smoothedNormal, hit.normal, 10f * Time.deltaTime);

            Quaternion slopeRotation =
                Quaternion.FromToRotation(transform.up, smoothedNormal) *
                transform.rotation;

            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation, 8f * Time.deltaTime);
        }
    }

    // ======================================================
    // STATE SYSTEM (UNCHANGED)
    // ======================================================
    public void SwitchState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // ======================================================
    // ATTACK (kept but not spammed)
    // ======================================================
    public void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange)
        {
            lastAttackTime = Time.time;
            // attack logic stays same
        }
    }

    // ======================================================
    // SEARCH (unchanged API)
    // ======================================================
    public void HearSound(Vector3 soundPos)
    {
        heardSoundPosition = soundPos;
        agent.speed = chaseSpeed;
        SwitchState(new EnemySearchState(this, soundPos));
    }

    public Transform GetNearestPatrolPoint()
    {
        Transform nearest = patrolPoints[0];
        float minDist = Mathf.Infinity;

        foreach (var p in patrolPoints)
        {
            float d = Vector3.Distance(transform.position, p.position);
            if (d < minDist)
            {
                minDist = d;
                nearest = p;
            }
        }

        return nearest;
    }

    void HandleAudioAndUI()
    {
        // moved from Update → reduced frequency indirectly via vision system
    }

    // ======================================================
    // GIZMOS (UNCHANGED)
    // ======================================================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (player == null) return;

        Vector3 origin = transform.position + Vector3.up * 3f;
        Vector3 target = player.position + Vector3.up * 2f;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin, (target - origin).normalized * Vector3.Distance(origin, target));
    }
}