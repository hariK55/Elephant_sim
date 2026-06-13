using UnityEngine;

public class EnemyChaseState : EnemyState
{
    public EnemyChaseState(EnemyAI enemy) : base(enemy) { }

    public override void Enter()
    {
       // Debug.Log("chasing");
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.chaseSpeed;
        enemy.animatorKumki.SetBool("isSearching", false);
        // SoundManager.Instance.StopMusic();
       
        
    }
    float repathDistance = 1f;

    float visionTimer;
    const float visionInterval = 0.15f;
    bool cachedCanSeePlayer = true;

    public override void Update()
    {
        if (Vector3.Distance(enemy.agent.destination,
                             enemy.player.position) > repathDistance)
        {
            enemy.agent.SetDestination(enemy.player.position);
        }

        float attackRangeSqr =
            enemy.attackRange * enemy.attackRange;

        if ((enemy.transform.position -
             enemy.player.position).sqrMagnitude <= attackRangeSqr)
        {
            enemy.SwitchState(new EnemyAttackState(enemy));
            return;
        }

        visionTimer += Time.deltaTime;

        if (visionTimer >= visionInterval)
        {
            visionTimer = 0f;

            if (!enemy.CanSeePlayer())
            {
                enemy.SwitchState(
                    new EnemySearchState(enemy, enemy.lastKnownPosition)
                );
            }
        }
    }

    public override void Exit() {
       
    }
}
