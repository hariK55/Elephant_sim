using UnityEngine;

public class EnemyIdleState : EnemyState
{
    public EnemyIdleState(EnemyAI enemy) : base(enemy)
    {
        
    }
    public override void Enter()
    {
        enemy.agent.isStopped = true;
        enemy.animatorKumki.SetFloat("speed", 0f);
    }

    public override void Exit()
    {
       
    }

    public override void Update()
    {
       
    }

    
}
