using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Animator animator;
    public float chaseSpeed = 6.5f;

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

    void Start()
    {
        animator = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation =false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        SwitchState(new EnemyPatrolState(this));
    }

    void Update()
    {
        currentState.Update();
        UpdateAnimator();
     //   Debug.Log("SpeedParam: " + animator.GetFloat("speed"));

    }

    void LateUpdate()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Quaternion rot = Quaternion.LookRotation(agent.velocity.normalized);
            transform.rotation =
                Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);
        }
    }


    public void SwitchState(EnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // ---------- SENSING ----------

    public bool CanSeePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > viewRadius) return false;
        if (Vector3.Angle(transform.forward, dir) > viewAngle / 2f) return false;
        if (Physics.Raycast(transform.position + Vector3.up, dir, dist, obstacleLayer)) return false;

        lastKnownPosition = player.position;
        return true;
    }

    public void HearSound(Vector3 soundPos)
    {
        heardSoundPosition = soundPos;
        SwitchState(new EnemySearchState(this, soundPos));
    }

    // ---------- PATROL HELPERS ----------

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

    void UpdateAnimator()
    {
        float speed01 = Mathf.Clamp01(
            agent.desiredVelocity.magnitude / chaseSpeed
        );

        animator.SetFloat("speed", speed01,0.15f,Time.deltaTime);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;

        Vector3 left = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + left * 20f);
        Gizmos.DrawLine(transform.position, transform.position + right * 20f);
    }
}
