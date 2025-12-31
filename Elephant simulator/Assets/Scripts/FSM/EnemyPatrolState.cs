using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyPatrolState : EnemyState
{
   
    private NavMeshAgent agent;

    private int patrolIndex;
    private int patrolDirection; // 1 = forward, -1 = backward
    private bool waiting;

    public EnemyPatrolState(EnemyAI enemy):base(enemy)
    {
        this.enemy = enemy;
        this.agent = enemy.agent;
    }

    public override void Enter()
    {
        Debug.Log("patrolling");
        patrolIndex = enemy.patrolIndex;
        patrolDirection = enemy.patrolDirection;
        waiting = false;

        agent.SetDestination(enemy.patrolPoints[patrolIndex].position);
    }

    public override void Update()
    {
        // Vision check handled here
        if (enemy.CanSeePlayer())
        {
            enemy.SwitchState(new EnemyChaseState(enemy));
            return;
        }

        if (waiting) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            enemy.StartCoroutine(WaitAndMove());
        }
    }

    IEnumerator WaitAndMove()
    {
        waiting = true;

        float waitTime = Random.Range(enemy.minWaitTime, enemy.maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        // 🔁 Reverse direction at patrol ends
        if (patrolIndex == enemy.patrolPoints.Length - 1)
            patrolDirection = -1;
        else if (patrolIndex == 0)
            patrolDirection = 1;

        patrolIndex += patrolDirection;

        // Save back to EnemyAI so other states know current patrol info
        enemy.patrolIndex = patrolIndex;
        enemy.patrolDirection = patrolDirection;

        agent.SetDestination(enemy.patrolPoints[patrolIndex].position);

        waiting = false;
    }

    public override void Exit()
    {
        enemy.StopAllCoroutines();
        waiting = false;
    }
}
