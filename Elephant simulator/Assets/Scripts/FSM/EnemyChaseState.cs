using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyAI enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log("chasing");
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.chaseSpeed;
        enemy.animatorKumki.SetBool("isSearching", false);
    }
    float repathDistance = 1f;
    public override void Update()
    {
       

        if (Vector3.Distance(enemy.agent.destination, enemy.player.position) > repathDistance)
        {
            enemy.agent.SetDestination(enemy.player.position);
        }

       // enemy.agent.SetDestination(enemy.player.position);

        float dist = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (dist <= enemy.attackRange)
        {
            enemy.SwitchState(new EnemyAttackState(enemy));
            return;
        }

        if (!enemy.CanSeePlayer())
        {
            enemy.SwitchState(new EnemySearchState(enemy, enemy.lastKnownPosition));
        }
    }

    public override void Exit() {
        
    }
}
