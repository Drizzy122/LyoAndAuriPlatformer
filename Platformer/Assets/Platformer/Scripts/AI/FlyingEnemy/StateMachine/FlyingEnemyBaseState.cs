using UnityEngine;

namespace Platformer
{
    public abstract class FlyingEnemyBaseState : IState
    {
        protected readonly FlyingEnemy flyingEnemy;
        protected readonly Animator animator;
        
        protected static readonly int FlyIdleHash = Animator.StringToHash("FlyingIdle");
        protected static readonly int FlyHash = Animator.StringToHash("FlyingMove");
        protected static readonly int AttackHash = Animator.StringToHash("FlyingAttack");
        protected static readonly int DieHash = Animator.StringToHash("Die");
        
        protected const float crossFadeDuration = 0.1f;

        protected FlyingEnemyBaseState(FlyingEnemy flyingEnemy, Animator animator)
        {
            this.flyingEnemy = flyingEnemy;
            this.animator = animator;
        }
        
        public virtual void OnEnter() {
            // noop
        }

        public virtual void Update() {
            // noop
        }

        public virtual void FixedUpdate() {
            // noop
        }

        public virtual void OnExit() {
            // noop
        }
    }
}