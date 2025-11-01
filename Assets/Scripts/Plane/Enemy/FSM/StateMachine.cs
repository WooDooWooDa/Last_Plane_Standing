namespace Plane.Enemy.FSM
{
    public class StateMachine
    {
        private State CurrentState { get; set; }

        public void Initialize(State startingState)
        {
            CurrentState = startingState;
            CurrentState.EnterState();
        }

        public void Update()
        {
            CurrentState.UpdateState();
            CurrentState.CheckTransition();
        }

        public void ChangeState(State newState)
        {
            CurrentState.ExitState();
            CurrentState = newState;
            CurrentState.EnterState();
        }
    }
}