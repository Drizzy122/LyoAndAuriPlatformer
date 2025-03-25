using UnityEngine;
using UnityEngine.AI;

namespace Platformer {
    public class EnemyKnockbackState : EnemyBaseState {
        readonly NavMeshAgent agent;
        readonly Transform player;

        private Vector3 knockbackTargetPosition;

        public EnemyKnockbackState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator) {
            this.agent = agent;
            this.player = player;
        }
        
        public override void OnEnter() {
            agent.speed = enemy.knockbackSpeed;
            agent.angularSpeed = 100000;
            agent.acceleration = 1000;

            knockbackTargetPosition = (enemy.transform.position - player.transform.position).normalized * 3;
            
            animator.CrossFade(IdleHash, crossFadeDuration);
            enemy.knockBackDirection = knockbackTargetPosition;
        }

        public override void OnExit()
        {
            enemy.knockBackDirection = Vector3.zero;
            agent.angularSpeed = enemy.turnSpeedToReturnTo;
        }
        
        public override void Update()
        {
            agent.speed = enemy.RunSpeed;
            agent.SetDestination(enemy.transform.position + knockbackTargetPosition);
            enemy.transform.LookAt(player);
        }
    }
}