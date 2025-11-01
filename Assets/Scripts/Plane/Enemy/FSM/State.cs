namespace Plane.Enemy.FSM
{
    public abstract class State
    {
        public State(EnemyPlane enemy, StateMachine stateMachine)
        {
            this.enemy = enemy;
            this.stateMachine = stateMachine;
        }

        public EnemyPlane enemy { get; set; }
        public StateMachine stateMachine { get; set; }

        public abstract void EnterState();
        public abstract void CheckTransition();
        public abstract void UpdateState();
        public abstract void ExitState();
    }
}