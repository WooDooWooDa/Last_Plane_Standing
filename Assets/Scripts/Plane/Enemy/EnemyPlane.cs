using System;
using Plane.Enemy.FSM;

namespace Plane.Enemy
{
    public class EnemyPlane : Plane
    {
        private StateMachine _stateMachine;
        private PatrolState _patrolState;
        
        protected override void Awake()
        {
            base.Awake();
            _stateMachine = new StateMachine();
            InitStates();
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        private void InitStates()
        {
            _patrolState = new PatrolState(this, _stateMachine, 10f);
            _stateMachine.Initialize(_patrolState);
        }
    }
}