public abstract class EnemyState
{
    protected EnemyAI enemy;

    public EnemyState(EnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}
