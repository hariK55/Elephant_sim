using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyAI enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log("chasing");
        enemy.agent.isStopped = false;
    }

    public override void Update()
    {
        enemy.agent.SetDestination(enemy.player.position);

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

    public override void Exit() { }
}
