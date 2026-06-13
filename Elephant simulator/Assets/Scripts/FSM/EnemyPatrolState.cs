using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyPatrolState : EnemyState
{
    private NavMeshAgent agent;

    private int patrolIndex;
    private int patrolDirection;
    private bool waiting;

    // --- OPTIMIZATION ---
    float visionTimer;
    const float visionInterval = 0.2f;
    bool cachedSeePlayer;

    bool waitRoutineRunning;

    public EnemyPatrolState(EnemyAI enemy) : base(enemy)
    {
        this.enemy = enemy;
        this.agent = enemy.agent;
    }

    public override void Enter()
    {
        agent.speed = 3f;

        enemy.animatorKumki.SetBool("isSearching", false);

        patrolIndex = enemy.patrolIndex;
        patrolDirection = enemy.patrolDirection;

        waiting = false;
        waitRoutineRunning = false;

        agent.SetDestination(enemy.patrolPoints[patrolIndex].position);
    }

    public override void Update()
    {
        // ---------------- THROTTLED VISION ----------------
        visionTimer += Time.deltaTime;

        if (visionTimer >= visionInterval)
        {
            visionTimer = 0f;
            cachedSeePlayer = enemy.CanSeePlayer();
        }

        if (cachedSeePlayer)
        {
            enemy.SwitchState(new EnemyChaseState(enemy));
            return;
        }

        if (waiting) return;

        // cache locals (reduces repeated property access)
        float remaining = agent.remainingDistance;
        bool pathPending = agent.pathPending;

        if (!pathPending && remaining <= agent.stoppingDistance)
        {
            if (!waitRoutineRunning)
            {
                enemy.StartCoroutine(WaitAndMove());
            }
        }
    }

    IEnumerator WaitAndMove()
    {
        waitRoutineRunning = true;
        waiting = true;

        float waitTime = Random.Range(enemy.minWaitTime, enemy.maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        // reverse patrol direction
        if (patrolIndex == enemy.patrolPoints.Length - 1)
            patrolDirection = -1;
        else if (patrolIndex == 0)
            patrolDirection = 1;

        patrolIndex += patrolDirection;

        enemy.patrolIndex = patrolIndex;
        enemy.patrolDirection = patrolDirection;

        agent.SetDestination(enemy.patrolPoints[patrolIndex].position);

        waiting = false;
        waitRoutineRunning = false;
    }

    public override void Exit()
    {
        enemy.StopAllCoroutines();

        waiting = false;
        waitRoutineRunning = false;
    }
}