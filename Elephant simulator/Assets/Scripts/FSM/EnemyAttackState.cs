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

       else
        {
            Debug.Log("Attack!");
            enemy.animatorKumki.SetTrigger("attack");
            SoundManager.Instance.PlaySfx(Sound.Trumpet, 0.5f);
            SoundManager.Instance.StopMusic();
            //Input.Instance.caught = true;
            GameManager.Instance.LoseGame(5f,"you're caught");
            ElephantAnimation.Instance.Caught();
            enemy.SwitchState(new EnemyIdleState(enemy));

           
        }
    }

    public override void Exit()
    {
        enemy.agent.isStopped = false;
    }
}
