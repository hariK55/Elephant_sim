using UnityEngine;

public class EnemyAttackState : EnemyState
{
    public EnemyAttackState(EnemyAI enemy) : base(enemy) { }

   
    public override void Enter()
    {
        enemy.agent.isStopped = true;
        ElephantAnimation.Instance.Caught();
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

        if (true)
        {
            Debug.Log("Attack!");
            enemy.animatorKumki.SetTrigger("attack");
            
            Input.Instance.caught = true;

            enemy.SwitchState(new EnemyIdleState(enemy));
           
        }
    }

    public override void Exit()
    {
        enemy.agent.isStopped = false;
    }
}
