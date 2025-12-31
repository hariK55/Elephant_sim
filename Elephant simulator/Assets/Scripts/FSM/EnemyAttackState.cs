using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(EnemyAI enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.agent.isStopped = true;
    }

    public override void Update()
    {
        enemy.transform.LookAt(enemy.player);

        float dist = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (dist > enemy.attackRange)
        {
            enemy.SwitchState(new EnemyChaseState(enemy));
            return;
        }

        if (Time.time >= enemy.lastAttackTime + enemy.attackCooldown)
        {
            Debug.Log("Attack!");
            enemy.lastAttackTime = Time.time;
        }
    }

    public override void Exit()
    {
        enemy.agent.isStopped = false;
    }
}
