using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class EnemySearchState : EnemyState
{
    Queue<Vector3> searchQueue = new Queue<Vector3>();

    public EnemySearchState(EnemyAI enemy, Vector3 center) : base(enemy)
    {
        GenerateSearchPoints(center);
       
    }

    public override void Enter()
    {
        enemy.agent.speed = 4f;
        enemy.animatorKumki.SetBool("isSearching", true);
        Debug.Log("searching");
        enemy.agent.isStopped = false;
        MoveNext();
    }

    public override void Update()
    {
        if (enemy.CanSeePlayer())
        {
            enemy.SwitchState(new EnemyChaseState(enemy));
            return;
        }

        if (!enemy.agent.pathPending && enemy.agent.remainingDistance < enemy.agent.stoppingDistance)
        {
            if (searchQueue.Count > 0)
                MoveNext();
            else
                ReturnToPatrol();
        }

    }

    void MoveNext()
    {
        if(searchQueue.Count!=0)
        enemy.agent.SetDestination(searchQueue.Dequeue());
    }

    void ReturnToPatrol()
    {
        Transform p = enemy.GetNearestPatrolPoint();
        enemy.patrolIndex = System.Array.IndexOf(enemy.patrolPoints, p);
        enemy.SwitchState(new EnemyPatrolState(enemy));
    }

    void GenerateSearchPoints(Vector3 center)
    {
        for (int i = 0; i < enemy.searchPoints; i++)
        {
            float angle = i * (360f / enemy.searchPoints);
            float radius = enemy.searchRadiusStep * (i + 1);

            Vector3 point = center + new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad) * radius
            );

            if (NavMesh.SamplePosition(point, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                searchQueue.Enqueue(hit.position);
        }
    }

   

    public override void Exit() {

       
    }
}
