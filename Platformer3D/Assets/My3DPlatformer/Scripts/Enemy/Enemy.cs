using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;
using Utilities;

namespace Platformer {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity {
        [SerializeField, Self] NavMeshAgent agent;
        [SerializeField, Self] PlayerDetector playerDetector;
        [SerializeField, Child] Animator animator;
        
        [SerializeField] float wanderRadius = 10f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float damageAmount = 10f;
        
        StateMachine stateMachine;
        
        CountdownTimer attackTimer;

        void OnValidate() => this.ValidateRefs();

        void Start() {
            attackTimer = new CountdownTimer(timeBetweenAttacks);
            
            stateMachine = new StateMachine();
            
            var wanderState = new EnemyWanderState(this, animator, agent, wanderRadius);
            var chaseState = new EnemyChaseState(this, animator, agent, playerDetector.Player);
            var attackState = new EnemyAttackState(this, animator, agent, playerDetector.Player);
            
            At(wanderState, chaseState, new FuncPredicate(() => playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !playerDetector.CanAttackPlayer()));

            stateMachine.SetState(wanderState);
        }
        
        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        void Update() {
            stateMachine.Update();
            attackTimer.Tick(Time.deltaTime);
        }
        
        void FixedUpdate() {
            stateMachine.FixedUpdate();
        }
        
        public void Attack() {
            if (attackTimer.IsRunning) return;
            AudioManager.instance.PlayOneShot(FMODEvents.instance.enemyAttack, this.transform.position);
            attackTimer.Start();

            // Access PlayerHealth and apply damage only if the player is not invulnerable
            var playerHealth = playerDetector.Player.GetComponent<PlayerHealth>();
            if (playerHealth != null) {
                if (!playerHealth.IsInvulnerable) { // Assuming you add a public IsInvulnerable property
                    playerHealth.TakeDamage(damageAmount);
                } else {
                    Debug.Log("Player is invulnerable. Attack did no damage.");
                }
            } else {
                Debug.LogWarning("PlayerHealth component not found on the player!");
            }
        }
    }
}